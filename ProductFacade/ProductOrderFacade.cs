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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public ProductOrderFacade(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<bool> UpdateProducts(IList<ProductUpdateDto> products)
        {
            if (products == null || products.Count == 0)
            {
                return false;
            }
            HttpClient httpClient = await GetClientWithAccessToken();
            if (httpClient != null)
            {
                string uri = _config.GetSection("CustomerOrderingUri").Value;
                if ((await httpClient.PostAsJsonAsync<IList<ProductUpdateDto>>(uri, products)).IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<HttpClient> GetClientWithAccessToken()
        {
            string orderUrl = _config.GetSection("CustomerOrderingUrl").Value;
            string authServerUrl = _config.GetSection("CustomerAuthServerUrl").Value;
            string clientSecret = _config.GetSection("ClientSecret").Value;
            string clientId = _config.GetSection("ClientId").Value;
            if (string.IsNullOrEmpty(authServerUrl)
                || string.IsNullOrEmpty(clientSecret)
                || string.IsNullOrEmpty(clientId)
                || string.IsNullOrEmpty(orderUrl))
            {
                return null;
            }
            var client = _httpClientFactory.CreateClient("CustomerOrderingAPI");
            var disco = await client.GetDiscoveryDocumentAsync(authServerUrl);
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = "order_api"
            });
            client.SetBearerToken(tokenResponse.AccessToken);
            return client;
        }
    }
}
