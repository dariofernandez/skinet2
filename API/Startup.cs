using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public class Startup
    {
        //DF
        //public IConfiguration Configuration { get; }
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //some content moved to ApplicationServicesExtensions class

            services.AddAutoMapper(typeof(MappingProfiles));

            services.AddControllers();

            //DF
            // lifetime is scoped (for the request)
            services.AddDbContext<StoreContext>(x =>
            x.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddApplicationServices();

            services.AddSwaggerDocumentation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // exceptions in Development Mode
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseMiddleware<ExceptionMiddleware>();

            //by DF 51. for error handling
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            //DF
            app.UseStaticFiles();

            app.UseAuthorization();

            // see extensions
            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
