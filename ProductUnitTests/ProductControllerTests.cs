using AutoMapper;
using CustomerProductService;
using CustomerProductService.Controllers;
using CustomerProductService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using ProductOrderFacade;
using ProductOrderFacade.Models;
using ProductRepository;
using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProductUnitTests
{
    public class ProductControllerTests
    {
        private IList<ProductDto> products;
        private ProductDto product1, product2, product3, product4;
        private ProductRepoModel productRepoModel;
        private ProductInfoRepoModel productInfoRepoModel;
        private List<String> repoBrands, repoCategories;
        private FakeProductRepository fakeRepo;
        private Mock<IProductRepository> mockRepo;
        private FakeProductOrderFacade fakeOrderFacade;
        private Mock<IProductOrderFacade> mockOrderFacade;
        private IMapper mapper;
        private ILogger<ProductController> logger;
        private ProductController controller;
        private bool updateProductSucceeds = true;
        private bool updateBrandsSucceeds = true;
        private bool updateCategoriesSucceeds = true;
        private bool getProductSucceeds = true;
        private bool getProductsSucceeds = true;
        private bool getProductInfoSucceeds = true;
        private bool facadeSucceeds = true;

        private void SetStandardProductsDto()
        {
            product1 = new ProductDto
            {
                ProductId = 1,
                Name = "Product 1",
                Description = "Description 1",
                Quantity = 1,
                BrandId = 1,
                Brand = "Brand 1",
                CategoryId = 1,
                Category = "Category 1",
                Price = 1.01
            };
            product2 = new ProductDto
            {
                ProductId = 2,
                Name = "Product 2",
                Description = "Description 2",
                Quantity = 2,
                BrandId = 2,
                Brand = "Brand 2",
                CategoryId = 2,
                Category = "Category 2",
                Price = 2.02
            };
            product3 = new ProductDto
            {
                ProductId = 3,
                Name = "Product 3",
                Description = "Description 3",
                Quantity = 3,
                BrandId = 1,
                Brand = "Brand 1",
                CategoryId = 2,
                Category = "Category 2",
                Price = 3.03
            };
            product4 = new ProductDto
            {
                ProductId = 4,
                Name = "Product 4",
                Description = "Description 4",
                Quantity = 4,
                BrandId = 2,
                Brand = "Brand 2",
                CategoryId = 1,
                Category = "Category 1",
                Price = 4.04
            };
            products = new List<ProductDto>()
            {
                product1, product2, product3,product4
            };
        }

        private void SetupProductRepoModel()
        {
            productRepoModel = new ProductRepoModel
            {
                ProductId = 1,
                Name = "Product 1",
                Description = "Description 1",
                Quantity = 1,
                BrandId = 1,
                Brand = "Brand 1",
                CategoryId = 1,
                Category = "Category 1",
                Price = 1.01
            };
        }

        private void SetupBrands()
        {
            repoBrands = new List<string>
            {
                "Brand 1", "Brand 2", "Brand 3"
            };
        }

        private void SetupCategories()
        {
            repoCategories = new List<string>
            {
                "Category 1", "Category 2", "Category 3"
            };
        }

        private void SetupProductInfoModel()
        {
            productInfoRepoModel = new ProductInfoRepoModel
            {
                Brands = repoBrands,
                Categories = repoCategories
            };
        }

        private void SetFakeRepo()
        {
            fakeRepo = new FakeProductRepository
            {
                ProductRepoModel = productRepoModel,
                Brands = repoBrands,
                Categories = repoCategories
            };
        }

        private void SetMapper()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            }).CreateMapper();
        }

        private void SetLogger()
        {
            logger = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
                .GetService<ILoggerFactory>()
                .CreateLogger<ProductController>();
        }

        private void SetMockCustomerRepo()
        {
            mockRepo = new Mock<IProductRepository>(MockBehavior.Strict);
            mockRepo.Setup(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()))
                .ReturnsAsync(updateProductSucceeds).Verifiable();
            mockRepo.Setup(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()))
                .ReturnsAsync(updateBrandsSucceeds).Verifiable();
            mockRepo.Setup(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()))
                .ReturnsAsync(updateCategoriesSucceeds).Verifiable();
            mockRepo.Setup(repo => repo.GetProduct(It.IsAny<int>()))
                .ReturnsAsync(getProductSucceeds?productRepoModel:null).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(getProductsSucceeds?new List<ProductRepoModel> { productRepoModel }:new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProductInfo()).Returns(Task.FromResult(productInfoRepoModel)).Verifiable();
            mockRepo.Setup(repo => repo.GetProductInfo()).Returns(Task.FromResult(productInfoRepoModel)).Verifiable();
        }

        private void SetMockOrderFacade()
        {
            mockOrderFacade = new Mock<IProductOrderFacade>(MockBehavior.Strict);
            mockOrderFacade.Setup(facade => facade.UpdateProducts(It.IsAny<List<ProductUpdateDto>>()))
                .ReturnsAsync(facadeSucceeds).Verifiable();
            
        }

        private void SetFakeFacade()
        {
            fakeOrderFacade = new FakeProductOrderFacade();
        }

        private void DefaultSetup()
        {
            SetStandardProductsDto();
            SetupProductRepoModel();
            SetupBrands();
            SetupCategories();
            SetupProductInfoModel();
            SetMapper();
            SetLogger();
        }

        private void SetupWithMocks()
        {
            DefaultSetup();
            SetMockCustomerRepo();
            SetMockOrderFacade();
            controller = new ProductController(logger, mockRepo.Object, mapper, mockOrderFacade.Object);
        }

        private void SetupWithFakes()
        {
            DefaultSetup();
            SetFakeRepo();
            SetFakeFacade();
            controller = new ProductController(logger, fakeRepo, mapper, fakeOrderFacade);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNull_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null, 
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptProductIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptBrandIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptCategoryIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptMinPriceZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.00, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptMaxPriceZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.00);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptBrandEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptCategoryEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithFakes_AllNullExceptSearchStringEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(fakeRepo.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(fakeRepo.Brands[i], info.Brands[i]);
            }
            Assert.Equal(fakeRepo.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(fakeRepo.Categories[i], info.Categories[i]);
            }
            Assert.Equal(fakeRepo.Brands, info.Brands);
            Assert.Equal(fakeRepo.Categories, info.Categories);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNull_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptProductIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptBrandIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptCategoryIdZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptMinPriceZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.00, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptMaxPriceZero_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.00);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptBrandEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptCategoryEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_AllNullExceptSearchStringEmpty_ReturnsProductInfo()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.Equal(productInfoRepoModel.Brands.Count, info.Brands.Count);
            for (int i = 0; i < info.Brands.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Brands[i], info.Brands[i]);
            }
            Assert.Equal(productInfoRepoModel.Categories.Count, info.Categories.Count);
            for (int i = 0; i < info.Categories.Count; i++)
            {
                Assert.Equal(productInfoRepoModel.Categories[i], info.Categories[i]);
            }
            Assert.Equal(productInfoRepoModel.Brands, info.Brands);
            Assert.Equal(productInfoRepoModel.Categories, info.Categories);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Once);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task MockPlaceholder()
        {
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }
    }
}
