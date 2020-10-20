

//===================================================== before caching =========================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {

        // ------- DI -----------------------------
        //private readonly StoreContext _context;
        //public ProductsController(StoreContext context)
        //{
        //    _context = context;
        //}
        // ------- DI Ends--------------------------

        // ------- DI Repository Pattern  ----------
        //private readonly IProductRepository _repo;
        //public ProductsController(IProductRepository repo)
        //{
        //    _repo = repo;
        //}
        // ------- DI Repository Ends--------------------------

        // ------- DI Repository & Specification Pattern  ----------
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IMapper mapper)
        {
            _productsRepo = productsRepo;
            _productTypeRepo = productTypeRepo;
            _productBrandRepo = productBrandRepo;
            _mapper = mapper;
        }


        // ------- DI Repository Ends--------------------------



        [HttpGet]
        //public async Task<ActionResult<List<Product>>> GetProducts()
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery]ProductSpecParams productParams)
        {
            //var products = await _context.Products.ToListAsync();
            //var products = await _repo.GetProductsAsync();
            //var products = await _productsRepo.ListAllAsync();

            var spec = new ProductsWithTypesAndBrandsSpecification(
                productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var totalItems = await _productsRepo.CountAsync(countSpec);

            var products = await _productsRepo.ListAsync(spec);

            var data = _mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);


            ////return products.Select(product => new ProductToReturnDto
            ////{
            ////    Id = product.Id,
            ////    Name = product.Name,
            ////    Description = product.Description,
            ////    PictureUrl = product.PictureUrl,
            ////    Price = product.Price,
            ////    ProductBrand = product.ProductBrand.Name,
            ////    ProductType = product.ProductType.Name
            ////}).ToList();
            ////return Ok(products);

            //return Ok(_mapper.Map<IReadOnlyList<Product>,
            //  IReadOnlyList<ProductToReturnDto>>(products));

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex,
                productParams.PageSize, totalItems, data));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]   // not needed, just for letting Swagger what to respond
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<Product>> GetProduct(int id)
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            //return await _context.Products.FindAsync(id);
            //return await _repo.GetProductByIdAsync(id);
            //return await _productsRepo.GetByIDAsync(id);

            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            // map the product to the Dto
            //return  await _productsRepo.GetEntityWithSpec(spec);
            var product = await _productsRepo.GetEntityWithSpec(spec);

            // use Data Transfer Object
            //return new ProductToReturnDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    Price = product.Price,
            //    ProductBrand = product.ProductBrand.Name,
            //    ProductType = product.ProductType.Name
            //};

            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }


        [HttpGet("brands")]
        public async Task<ActionResult<List<Product>>> GetProductBrands()
        {
            //return Ok(await _repo.GetProductBrandsAsync());
            return Ok(await _productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<Product>>> GetProductTypes()
        {
            //return Ok(await _repo.GetProductTypesAsync());
            return Ok(await _productTypeRepo.ListAllAsync());
        }

    }
}

//============================================= Caching does not work yet ===========================
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using API.Dtos;
//using API.Errors;
//using API.Helpers;
//using AutoMapper;
//using Core.Entities;
//using Core.Interfaces;
//using Core.Specifications;
//using Infrastructure.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace API.Controllers
//{
//    public class ProductsController : BaseApiController
//    {
//        private readonly IGenericRepository<Product> _productsRepo;
//        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
//        private readonly IGenericRepository<ProductType> _productTypeRepo;
//        private readonly IMapper _mapper;

//        public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductBrand> productBrandRepo, IGenericRepository<ProductType> productTypeRepo, IMapper mapper)
//        {
//            _mapper = mapper;
//            _productTypeRepo = productTypeRepo;
//            _productBrandRepo = productBrandRepo;
//            _productsRepo = productsRepo;

//        }

//        [Cached(600)]
//        [HttpGet]
//        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
//            [FromQuery]ProductSpecParams productParams)
//        {
//            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

//            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

//            var totalItems = await _productsRepo.CountAsync(countSpec);

//            var products = await _productsRepo.ListAsync(spec);

//            var data = _mapper
//                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

//            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
//        }

//        [Cached(600)]
//        [HttpGet("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
//        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
//        {
//            var spec = new ProductsWithTypesAndBrandsSpecification(id);

//            var product = await _productsRepo.GetEntityWithSpec(spec);

//            if (product == null) return NotFound(new ApiResponse(404));

//            return _mapper.Map<Product, ProductToReturnDto>(product);
//        }

//        [HttpGet("brands")]
//        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
//        {
//            return Ok(await _productBrandRepo.ListAllAsync());
//        }



//        [Cached(1000)]
//        [HttpGet("types")]
//        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
//        {
//            return Ok(await _productTypeRepo.ListAllAsync());
//        }
//    }
//}
