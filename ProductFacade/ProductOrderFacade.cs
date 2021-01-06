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
        private readonly IHttpHandler _handler;
        private string customerAuthUrl;
        private string orderingApi;
        private string orderingScope;
        private string orderingUri;

        public ProductOrderFacade(IConfiguration config, IHttpHandler handler)
        {
            _handler = handler;
            if (config != null)
            {
                customerAuthUrl = config.GetSection("CustomerAuthServerUrlKey").Value;
                orderingApi = config.GetSection("CustomerOrderingAPIKey").Value;
                orderingScope = config.GetSection("CustomerOrderingScopeKey").Value;
                orderingUri = config.GetSection("CustomerOrderingUri").Value;
            }
        }

        public async Task<bool> UpdateProducts(IList<ProductUpdateDto> products)
        {
            if (products == null || products.Count == 0 || !ValidConfigStrings())
            {
                return false;
            }
            HttpClient httpClient = await _handler.GetClient(customerAuthUrl, orderingApi, orderingScope);
            if (httpClient != null)
            {
                var result = await httpClient.PostAsJsonAsync<IList<ProductUpdateDto>>(orderingUri, products);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidConfigStrings()
        {
            return !string.IsNullOrEmpty(customerAuthUrl)
                    && !string.IsNullOrEmpty(orderingApi)
                    && !string.IsNullOrEmpty(orderingScope)
                    && !string.IsNullOrEmpty(orderingUri);
        }
    }
}
