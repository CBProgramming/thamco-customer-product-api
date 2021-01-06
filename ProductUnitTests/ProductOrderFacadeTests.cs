using HttpManager;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using ProductOrderFacade;
using ProductOrderFacade.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProductUnitTests
{
    public class ProductOrderFacadeTests
    {
        public HttpClient client;
        public Mock<IHttpClientFactory> mockFactory;
        public Mock<HttpClient> mockClient;
        public Mock<HttpMessageHandler> mockHandler;
        public IProductOrderFacade facade;
        private IConfiguration config;
        private IList<ProductUpdateDto> products;
        private Mock<IHttpHandler> mockHttpHandler;
        private string orderingUriValue = "/api/Product/";
        private string customerAuthServerUrlKeyValue = "CustomerAuthServerUrl";
        private string orderingApiKeyValue = "CustomerOrderingAPI";
        private string orderingScopeKeyValue = "CustomerOrderingScope";
        Uri expectedUri = new Uri("http://test/api/Product/");

        private void SetupOrder()
        {
            products = new List<ProductUpdateDto>()
            {
                new ProductUpdateDto
                {
                    ProductId = 1,
                    Name = "Product 1",
                    Quantity = 1,
                    Price = 1.01
                },
                new ProductUpdateDto
                {
                    ProductId = 2,
                    Name = "Product 2",
                    Quantity = 2,
                    Price = 2.02
                },
                new ProductUpdateDto
                {
                    ProductId = 3,
                    Name = "Product 3",
                    Quantity = 3,
                    Price = 3.03
                }
            };
        }

        private void SetupConfig(string orderingUri = null, string customerAuthUrlKey = null, string? orderingAPIKey = null,
            string? orderingScope = null)
        {
            var myConfiguration = new Dictionary<string, string>
             {
                {"CustomerAuthServerUrlKey", customerAuthUrlKey??customerAuthServerUrlKeyValue},
                {"CustomerOrderingAPIKey", orderingAPIKey??orderingApiKeyValue},
                {"CustomerOrderingScopeKey", orderingScope??orderingScopeKeyValue},
                {"CustomerOrderingUri" , orderingUri?? orderingUriValue },
                {"ClientId", "ClientId"},
                {"ClientSecret", "ClientSecret"}
            };
            config = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }

        private void SetMockMessageHandler(HttpResponseMessage expected)
        {
            mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expected)
                .Verifiable();
        }

        private void SetupRealHttpClient(HttpResponseMessage expected)
        {
            client = new HttpClient(mockHandler.Object);
            client.BaseAddress = new Uri("http://test");

        }

        private void SetupHttpFactoryMock(HttpClient client)
        {
            mockFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client).Verifiable();
        }

        private void SetupHttpHandlerMock()
        {
            mockHttpHandler = new Mock<IHttpHandler>(MockBehavior.Strict);
            mockHttpHandler.Setup(f => f.GetClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(client)).Verifiable();
        }

        private void DefaultSetup(HttpStatusCode statusCode)
        {
            SetupOrder();
            var expectedResult = new HttpResponseMessage
            {
                StatusCode = statusCode
            };
            SetMockMessageHandler(expectedResult);
            SetupRealHttpClient(expectedResult);
            SetupHttpFactoryMock(client);
            SetupConfig();
            SetupHttpHandlerMock();
            facade = new ProductOrderFacade.ProductOrderFacade(config, mockHttpHandler.Object);
            //SetupConfig();
        }


        [Fact]
        public async Task UpdateProducts_ShouldReturnTrue()
        {
            //Arrange
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(true == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(customerAuthServerUrlKeyValue, orderingApiKeyValue,
                orderingScopeKeyValue), Times.Once);
        }

        [Fact]
        public async Task UpdateProducts_NotFound_ShouldReturnFalse()
        {
            //Arrange
            DefaultSetup(HttpStatusCode.NotFound);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(customerAuthServerUrlKeyValue, orderingApiKeyValue,
                orderingScopeKeyValue), Times.Once);
        }

        [Fact]
        public async Task UpdateProducts_Null_ShouldReturnFalse()
        {
            //Arrange
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(null);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_EmptyList_ShouldReturnFalse()
        {
            //Arrange
            DefaultSetup(HttpStatusCode.OK);
            products = new List<ProductUpdateDto>();

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_UriNull_ShouldReturnFalse()
        {
            //Arrange
            orderingUriValue = null;
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_UriEmpty_ShouldReturnFalse()
        {
            //Arrange
            orderingUriValue = "";
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_AuthKeyNull_ShouldReturnFalse()
        {
            //Arrange
            customerAuthServerUrlKeyValue = null;
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_AuthKeyEmpty_ShouldReturnFalse()
        {
            //Arrange
            customerAuthServerUrlKeyValue = "";
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_ApiKeyNull_ShouldReturnFalse()
        {
            //Arrange
            orderingApiKeyValue = null;
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_ApiKeyEmpty_ShouldReturnFalse()
        {
            //Arrange
            orderingApiKeyValue = "";
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_ScopeKeyNull_ShouldReturnFalse()
        {
            //Arrange
            orderingScopeKeyValue = null;
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProducts_ScopeKeyEmpty_ShouldReturnFalse()
        {
            //Arrange
            orderingScopeKeyValue = "";
            DefaultSetup(HttpStatusCode.OK);

            //Act
            var result = await facade.UpdateProducts(products);

            //Assert
            Assert.True(false == result);
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                 (req => req.Method == HttpMethod.Get), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Post), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Put), ItExpr.IsAny<CancellationToken>());
            mockHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.Is<HttpRequestMessage>
                (req => req.Method == HttpMethod.Delete), ItExpr.IsAny<CancellationToken>());
            mockHttpHandler.Verify(m => m.GetClient(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }
    }
}
