﻿using System.IO;
using System.Threading.Tasks;
using API.Errors;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;


namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly string _whSecret;   // from web hook from Stripe
        private readonly ILogger<IPaymentService> _logger;

        public PaymentsController(IPaymentService paymentService, ILogger<IPaymentService> logger, IConfiguration config)
        {
            _logger = logger;
            _paymentService = paymentService;
            _whSecret = config.GetSection("StripeSettings:WhSecret").Value;
        }


        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return basket;
        }


        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            // to confirm that the payment from Stripe was successful
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", intent.Id);
                    // update order with new status
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received: ", order.Id);
                    break;

                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", intent.Id);
                    // update order
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Payment Failed: ", order.Id);
                    break;
            }
            // Stripe needs this for result: (otherwise it keeps sending the event)
            return new EmptyResult();
        }

    }
}
