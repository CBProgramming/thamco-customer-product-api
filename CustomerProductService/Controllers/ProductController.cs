using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CustomerProductService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductRepository;
using ProductRepository.Data;

namespace CustomerProductService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger, IProductRepository productRepo, IMapper mapper)
        {
            _logger = logger;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? productId, int? brandId, int? categoryId, string? brand, string? category, string? searchString,
            double? minPrice, double? maxPrice)
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
                return Ok(_mapper.Map<List<ProductInfoDto>>(await _productRepo.GetProductInfo()));
            }
            else
            {
                return Ok(_mapper.Map<List<ProductDto>>(await _productRepo.GetProducts(productId, brandId,
                categoryId, brand, category, searchString, minPrice, maxPrice)));
            }
        }

        [HttpPost]
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
            var productsToRetry = new List<ProductDto>();
            foreach (ProductDto product in products)
            {
                if (await _productRepo.ProductExists(product.ProductId))
                {
                    if (!await _productRepo.EditProduct(_mapper.Map<ProductRepoModel>(product)))
                    {
                        productsToRetry.Add(product);
                    }
                }
                else
                {
                    if (!await _productRepo.NewProduct(_mapper.Map<ProductRepoModel>(product)))
                    {
                        productsToRetry.Add(product);
                    }
                }
            }
            if (productsToRetry.Count == 0)
            {
                return Ok();
            }
            else if (productsToRetry.Count == products.Count)
            {
                return NotFound();
            }
            else
            {
                //as this only occurs if a partial number of products doesn't post
                //there is no risk of an infinite loop as the retry list is always smaller
                //this could still be very problematic if the connection is poor and the list is long
                return await Create(productsToRetry);
            }
        }
    }
}
