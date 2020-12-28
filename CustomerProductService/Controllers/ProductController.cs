using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerProductService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductOrderFacade;
using ProductOrderFacade.Models;
using ProductRepository;
using ProductRepository.Data;

namespace CustomerProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;
        private readonly IProductOrderFacade _facade;

        public ProductController(ILogger<ProductController> logger, IProductRepository productRepo, IMapper mapper,
            IProductOrderFacade facade)
        {
            _logger = logger;
            _productRepo = productRepo;
            _mapper = mapper;
            _facade = facade;
        }

        [HttpGet]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> Get(int? productId, int? brandId, int? categoryId, string? brand, 
            string? category, string? searchString, double? minPrice, double? maxPrice)
        {
            //if nothing get a list of everything
            if ((productId == null || productId < 1)
                && (brandId == null || brandId < 1)
                && (categoryId == null || categoryId < 1)
                && string.IsNullOrEmpty(brand)
                && string.IsNullOrEmpty(category)
                && string.IsNullOrEmpty(searchString)
                && (minPrice == null || minPrice < 0.01)
                && (maxPrice == null || maxPrice < 0.01))
            {
                return Ok(_mapper.Map<ProductInfoDto>(await _productRepo.GetProductInfo()));
            }
            else if (productId != null || productId > 0)
            {
                return Ok(_mapper.Map<ProductDto>(await _productRepo.GetProduct(productId??0)));
            }
            else
            {
                return Ok(_mapper.Map<List<ProductDto>>(await _productRepo.GetProducts(brandId,
                categoryId, brand, category, searchString, minPrice, maxPrice)));
            }
        }

        [HttpPost]
        [Authorize(Policy = "StaffProductAPIOnly")]
        public async Task<IActionResult> Create(IList<ProductDto> products)
        {
            if (products == null || products.Count < 1)
            {
                return UnprocessableEntity();
            }
            foreach (ProductDto product in products)
            {
                if (product == null)
                {
                    return UnprocessableEntity();
                }
            }
            if (! await _productRepo.UpdateBrands(_mapper.Map<List<ProductRepoModel>>(products))
                || ! await _productRepo.UpdateCategories(_mapper.Map<List<ProductRepoModel>>(products)))
            {
                return NotFound();
            }
            if (await _productRepo.UpdateProducts(_mapper.Map<List<ProductRepoModel>>(products)))
            {
                if (! await _facade.UpdateProducts(_mapper.Map<List<ProductUpdateDto>>(products)))
                {
                    //record to local db to try later
                }
                return Ok();
            }
            return NotFound();
        }
    }
}
