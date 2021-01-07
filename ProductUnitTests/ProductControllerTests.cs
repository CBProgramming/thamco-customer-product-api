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
        private List<ProductDto> products;
        private IList<ProductRepoModel> repoProducts;
        private ProductDto product1, product2, product3, product4;
        private ProductRepoModel productRepoModel, productRepoModel1, 
            productRepoModel2, productRepoModel3, productRepoModel4;
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

        private void SetupProductRepoModels()
        {
            productRepoModel1 = new ProductRepoModel
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
            productRepoModel2 = new ProductRepoModel
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
            productRepoModel3 = new ProductRepoModel
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
            productRepoModel4 = new ProductRepoModel
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
            repoProducts = new List<ProductRepoModel>()
            {
                productRepoModel1, productRepoModel2, productRepoModel3,productRepoModel4
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
                RepoProducts = repoProducts,
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
            mockRepo.Setup(repo => repo.GetProducts(It.IsAny<int>(), null, null, null, null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, It.IsAny<int>(), null, null, null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, It.IsAny<string>(), null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, null, It.IsAny<string>(), null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, null, null, It.IsAny<double>(), null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, null, null, null, It.IsAny<double>()))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, It.IsAny<string>(), It.IsAny<string>(), null, null, null))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, null, It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(getProductsSucceeds ? new List<ProductRepoModel> { productRepoModel } : new List<ProductRepoModel>()).Verifiable();
            mockRepo.Setup(repo => repo.GetProducts(null, null, null, null, null, null, null))
                .ReturnsAsync(new List<ProductRepoModel>()).Verifiable();
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
            SetupProductRepoModels();
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
        public async Task GetProducts_WithFakes_AllNull_RepoFails_ReturnsEmptyList()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.RepoSucceeds = false;

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var info = obj.Value as ProductInfoDto;
            Assert.NotNull(info);
            Assert.True(info.Brands.Count == 0);
            Assert.True(info.Categories.Count == 0);
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
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_OtherOptionalParametersNull_ReturnsProduct()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_OtherOptionalParametersNull_RepoFails_ReturnsNotFound()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.RepoSucceeds = false;

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_OtherOptionalParametersNull_ReturnsNotFound()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ZeroBrandId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ZeroBrandId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidBrandId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidBrandId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ZeroCategoryId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ZeroCategoryId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidCategoryId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidCategoryId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_EmptyBrand_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_EmptyBrand_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidBrand_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidBrand_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_EmptyCategory_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_EmptyCategory_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidCategory_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidCategory_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_EmptySearchString_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_EmptySearchString_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidSearchString_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "Search String", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidSearchString_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "Search String", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ZeroMinPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ZeroMinPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidMinPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.01, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidMinPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.01, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ZeroMaxPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ZeroMaxPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatExists_ValidMaxPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
        }

        [Fact]
        public async Task GetProducts_WithFakes_ValidProductIdThatDoesntExist_ValidMaxPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_OtherOptionalParametersNull_ReturnsProduct()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_OtherOptionalParametersNull_ReturnsNotFound()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ZeroBrandId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ZeroBrandId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: 0, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidBrandId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidBrandId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ZeroCategoryId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ZeroCategoryId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: 0, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidCategoryId_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidCategoryId_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_EmptyBrand_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_EmptyBrand_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: "",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidBrand_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidBrand_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_EmptyCategory_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_EmptyCategory_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: "", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidCategory_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidCategory_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_EmptySearchString_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_EmptySearchString_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidSearchString_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "Search String", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidSearchString_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "Search String", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ZeroMinPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ZeroMinPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidMinPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.01, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidMinPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 0.01, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ZeroMaxPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ZeroMaxPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatExists_ValidMaxPrice_ReturnsProductPrioritisingProductId()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 1, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var product = obj.Value as ProductDto;
            Assert.NotNull(product);
            Assert.Equal(productRepoModel.ProductId, product.ProductId);
            Assert.Equal(productRepoModel.Name, product.Name);
            Assert.Equal(productRepoModel.Description, product.Description);
            Assert.Equal(productRepoModel.Quantity, product.Quantity);
            Assert.Equal(productRepoModel.BrandId, product.BrandId);
            Assert.Equal(productRepoModel.Brand, product.Brand);
            Assert.Equal(productRepoModel.CategoryId, product.CategoryId);
            Assert.Equal(productRepoModel.Category, product.Category);
            Assert.Equal(productRepoModel.Price, product.Price);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(1), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_ValidProductIdThatDoesntExist_ValidMaxPrice_ReturnsNotFoundPrioritisingProductId()
        {
            //Arrange
            getProductSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: 2, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var notFound = await result as NotFoundResult;
            Assert.NotNull(notFound);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(2), Times.Once);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);

        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidBrandId_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.Equal(1, product.BrandId);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidBrandId_RepoFails_ReturnsEmptyList()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.RepoSucceeds = false;

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_InvalidBrandId_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: 99, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidCategoryId_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.CategoryId == 1);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_InvalidCategoryId_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 99, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidBrand_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.Equal("Brand 1", product.Brand);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_InvalidBrand_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 99",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidCategory_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.Equal("Category 1", product.Category);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_InvalidCategory_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "Category 99", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidSearchOnName_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "uct 1", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.Name.Contains("uct 1") || product.Description.Contains("uct 1"));
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidSearchOnDescription_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "tion 1", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.Name.Contains("tion 1") || product.Description.Contains("tion 1"));
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_InvalidSearch_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "abc", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidMinPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 2, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.Price >= 2);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_TooHighMinPrice_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 9999999, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidMaxPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 2);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.Price <= 2);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_MaxPriceTooLow_ReturnsNoProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
        }

        //I would test all possible combinations here but unfortunately I don't have the time available
        //I instead chose to test a reasonable range of combinations
        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidBrandIdAndCategoryId_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.Equal(1, product.BrandId);
                Assert.Equal(1, product.CategoryId);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidBrandAndCategory_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 1",
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.Equal("Brand 1", product.Brand);
                Assert.Equal("Category 1", product.Category);
            }
        }

        [Fact]
        public async Task GetProducts_WithFakes_NullProductId_ValidSearchStringAndMinPriceAndMaxPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithFakes();

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: 1, brand: null,
                category: null, searchString: "uct", minPrice: 1, maxPrice: 3);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            foreach (ProductDto product in products)
            {
                Assert.True(product.Name.Contains("uct") || product.Description.Contains("uct"));
                Assert.True(product.Price >= 1);
                Assert.True(product.Price <= 3);
            }
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidBrandId_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(1, null, null, null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_InvalidBrandId_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: 99, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(99, null, null, null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidCategoryId_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, 1, null, null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_InvalidCategoryId_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: 99, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, 99, null, null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidBrand_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 1",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, "Brand 1", null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_InvalidBrand_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 99",
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, "Brand 99", null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidCategory_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, "Category 1", null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_InvalidCategory_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: "Category 99", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, "Category 99", null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidSearchOnName_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "uct 1", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, "uct 1", null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidSearchOnDescription_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "tion 1", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, "tion 1", null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_InvalidSearch_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "abc", minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, "abc", null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidMinPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 2, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, null, 2, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_TooHighMinPrice_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: 9999999, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, null, 9999999, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidMaxPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 2);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, null, null, 2), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_MaxPriceTooLow_ReturnsNoProducts()
        {
            //Arrange
            getProductsSucceeds = false;
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: 0.01);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count == 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, null, null, 0.01), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        //I would test all possible combinations here but unfortunately I don't have the time available
        //I instead chose to test a reasonable range of combinations
        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidBrandIdAndCategoryId_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: 1, categoryId: 1, brand: null,
                category: null, searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(1, 1, null, null, null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidBrandAndCategory_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: "Brand 1",
                category: "Category 1", searchString: null, minPrice: null, maxPrice: null);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, "Brand 1", "Category 1", null, null, null), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task GetProducts_WithMocks_NullProductId_ValidSearchStringAndMinPriceAndMaxPrice_ReturnsProducts()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Get(productId: null, brandId: null, categoryId: null, brand: null,
                category: null, searchString: "uct", minPrice: 1, maxPrice: 3);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkObjectResult;
            Assert.NotNull(obj);
            var products = obj.Value as List<ProductDto>;
            Assert.NotNull(products);
            Assert.True(products.Count > 0);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(null, null, null, null, "uct", 1, 3), Times.Once);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);
        }

        [Fact]
        public async Task CreateProducts_WithFakes_ValidProducts_ShouldOk()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.Brands = new List<string>();
            fakeRepo.Categories = new List<string>();
            fakeRepo.RepoProducts = new List<ProductRepoModel>();

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkResult;
            Assert.NotNull(obj);
            Assert.Equal(products.Count, fakeRepo.RepoProducts.Count);
            for (int i = 0; i < fakeRepo.RepoProducts.Count; i++)
            {
                Assert.Equal(fakeRepo.RepoProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(fakeRepo.RepoProducts[i].Name, products[i].Name);
                Assert.Equal(fakeRepo.RepoProducts[i].Description, products[i].Description);
                Assert.Equal(fakeRepo.RepoProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(fakeRepo.RepoProducts[i].Brand, products[i].Brand);
                Assert.Equal(fakeRepo.RepoProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(fakeRepo.RepoProducts[i].Category, products[i].Category);
                Assert.Equal(fakeRepo.RepoProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(fakeRepo.RepoProducts[i].Price, products[i].Price);
            }
            Assert.Equal(2, fakeRepo.Categories.Count);
            Assert.True(fakeRepo.Categories.Contains("Category 1"));
            Assert.True(fakeRepo.Categories.Contains("Category 2"));
            Assert.Equal(2, fakeRepo.Brands.Count);
            Assert.True(fakeRepo.Brands.Contains("Brand 1"));
            Assert.True(fakeRepo.Brands.Contains("Brand 2"));
        }

        [Fact]
        public async Task CreateProducts_WithFakes_ValidProducts_RepoFails_ShouldNotFound()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.RepoSucceeds = false;
            fakeRepo.Brands = new List<string>();
            fakeRepo.Categories = new List<string>();
            fakeRepo.RepoProducts = new List<ProductRepoModel>();

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as NotFoundResult;
            Assert.NotNull(obj);
            Assert.True(fakeRepo.RepoProducts.Count == 0);
            Assert.True(fakeRepo.Brands.Count == 0);
            Assert.True(fakeRepo.Categories.Count == 0);
        }

        [Fact]
        public async Task CreateProducts_WithFakes_NullProducts_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.Brands = new List<string>();
            fakeRepo.Categories = new List<string>();
            fakeRepo.RepoProducts = new List<ProductRepoModel>();
            products = null;

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
            Assert.True(fakeRepo.RepoProducts.Count == 0);
            Assert.True(fakeRepo.Brands.Count == 0);
            Assert.True(fakeRepo.Categories.Count == 0);
        }

        [Fact]
        public async Task CreateProducts_WithFakes_EmptyProducts_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.Brands = new List<string>();
            fakeRepo.Categories = new List<string>();
            fakeRepo.RepoProducts = new List<ProductRepoModel>();
            products = new List<ProductDto>();

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
            Assert.True(fakeRepo.RepoProducts.Count == 0);
            Assert.True(fakeRepo.Brands.Count == 0);
            Assert.True(fakeRepo.Categories.Count == 0);
        }

        [Fact]
        public async Task CreateProducts_WithFakes_OneProductIsNull_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithFakes();
            fakeRepo.Brands = new List<string>();
            fakeRepo.Categories = new List<string>();
            fakeRepo.RepoProducts = new List<ProductRepoModel>();
            products[1] = null;

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
            Assert.True(fakeRepo.RepoProducts.Count == 0);
            Assert.True(fakeRepo.Brands.Count == 0);
            Assert.True(fakeRepo.Categories.Count == 0);
        }

        [Fact]
        public async Task CreateProducts_WithMocks_ValidProducts_ShouldOk()
        {
            //Arrange
            SetupWithMocks();

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as OkResult;
            Assert.NotNull(obj);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Once);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Once);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Once);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Once);

        }

        [Fact]
        public async Task CreateProducts_WithMocks_NullProducts_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithMocks();
            products = null;

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);

        }

        [Fact]
        public async Task CreateProducts_WithMocks_EmptyProducts_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithMocks();
            products = new List<ProductDto>();

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
            mockRepo.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateBrands(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.UpdateCategories(It.IsAny<IList<ProductRepoModel>>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProduct(It.IsAny<int>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
            mockRepo.Verify(repo => repo.GetProductInfo(), Times.Never);
            mockOrderFacade.Verify(repo => repo.UpdateProducts(It.IsAny<IList<ProductUpdateDto>>()), Times.Never);

        }

        [Fact]
        public async Task CreateProducts_WithMocks_OneProductIsNull_ShouldUnprocessableEntity()
        {
            //Arrange
            SetupWithMocks();
            products[1] = null;

            //Act
            var result = controller.Create(products);

            //Assert 
            Assert.NotNull(result);
            var obj = await result as UnprocessableEntityResult;
            Assert.NotNull(obj);
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
