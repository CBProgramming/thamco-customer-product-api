using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerProductService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomerProductService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Task<IActionResult> Get(int? productId, string? searchTerm)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Create(ProductDto product)
        {
            throw new NotImplementedException();
        }
    }
}
