using HttpManager;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using ProductOrderFacade.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductOrderFacade
{
    public class ProductOrderFacade : IProductOrderFacade
    {
        private readonly IConfiguration _config;
        private readonly IHttpHandler _handler;

        public ProductOrderFacade(IConfiguration config, IHttpHandler handler)
        {
            _config = config;
            _handler = handler;
        }

        public async Task<bool> UpdateProducts(IList<ProductUpdateDto> products)
        {
            if (products == null || products.Count == 0)
            {
                return false;
            }
            HttpClient httpClient = await _handler.GetClient("CustomerOrderingUrl", "CustomerOrderingAPI", "CustomerOrderingScope");
            if (httpClient != null)
            {
                string uri = _config.GetSection("CustomerOrderingUri").Value;
                var result = await httpClient.PostAsJsonAsync<IList<ProductUpdateDto>>(uri, products);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
