using AutoMapper;
using CustomerProductService;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductData;
using ProductRepository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProductUnitTests
{

    public class ProductRepoTests
    {
        public ProductRepoModel productRepoModel1, productRepoModel2;
        public IList<ProductRepoModel> productRepoModels;
        public IMapper mapper;
        public IQueryable<Product> dbProducts;
        public Product dbProduct1, dbProduct2, dbProduct3, dbProduct4;
        public Mock<DbSet<Product>> mockProducts;
        public IQueryable<Brand> dbBrands;
        public Brand dbBrand1, dbBrand2;
        public Mock<DbSet<Brand>> mockBrands;
        public IQueryable<Category> dbCategories;
        public Category dbCategory1, dbCategory2;
        public Mock<DbSet<Category>> mockCategories;
        public Mock<ProductDb> mockDbContext;
        public ProductRepository.ProductRepository repo;

        private void SetupProductRepoModel()
        {
            productRepoModel1 = new ProductRepoModel
            {
                ProductId = 1,
                Name = "Product 5",
                Description = "Description 5",
                Quantity = 5,
                BrandId = 2,
                Brand = "Brand 2",
                CategoryId = 2,
                Category = "Category 2",
                Price = 5.05
            };
            productRepoModel2 = new ProductRepoModel
            {
                ProductId = 5,
                Name = "Product 6",
                Description = "Description 6",
                Quantity = 6,
                BrandId = 1,
                Brand = "Brand 1",
                CategoryId = 1,
                Category = "Category 1",
                Price = 6.06
            };
        }

        private void SetupProductRepoModels()
        {
            SetupProductRepoModel();
            productRepoModels = new List<ProductRepoModel>
            {
                productRepoModel1,
                productRepoModel2
            };
        }

        private void SetupDbProduct()
        {
            dbProduct1 = new Product
            {
                ProductId = 1,
                Name = "Product 1",
                Description = "Description 1",
                Quantity = 1,
                BrandId = 1,
                CategoryId = 1,
                Price = 1.01
            };
            dbProduct2 = new Product
            {
                ProductId = 2,
                Name = "Product 2",
                Description = "Description 2",
                Quantity = 2,
                BrandId = 2,
                CategoryId = 2,
                Price = 2.02
            };
            dbProduct3 = new Product
            {
                ProductId = 3,
                Name = "Product 3",
                Description = "Description 3",
                Quantity = 3,
                BrandId = 1,
                CategoryId = 2,
                Price = 3.03
            };
            dbProduct4 = new Product
            {
                ProductId = 4,
                Name = "Product 4",
                Description = "Description 4",
                Quantity = 4,
                BrandId = 2,
                CategoryId = 1,
                Price = 4.04
            };
        }
        private void SetupDbBrand()
        {
            dbBrand1 = new Brand
            {
                BrandId = 1,
                BrandName = "Brand 1"
            };
            dbBrand2 = new Brand
            {
                BrandId = 2,
                BrandName = "Brand 2"
            };
        }

        private void SetupDbCategory()
        {
            dbCategory1 = new Category
            {
                CategoryId = 1,
                CategoryName = "Category 1"
            };
            dbCategory2 = new Category
            {
                CategoryId = 2,
                CategoryName = "Category 2"
            };
        }

        private void SetupMapper()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            }).CreateMapper();
        }

        private void SetupDbProducts()
        {
            SetupDbProduct();
            dbProducts= new List<Product>
            {
                dbProduct1,
                dbProduct2,
                dbProduct3,
                dbProduct4
            }.AsQueryable();
        }

        private void SetupMockProducts()
        {
            mockProducts = new Mock<DbSet<Product>>();
            mockProducts.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(dbProducts.Provider);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(dbProducts.Expression);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(dbProducts.ElementType);
            mockProducts.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(dbProducts.GetEnumerator());
        }

        private void SetupDbCategories()
        {
            SetupDbCategory();
            dbCategories = new List<Category>
            {
                dbCategory1,
                dbCategory2
            }.AsQueryable();
        }

        private void SetupMockCategories()
        {
            mockCategories = new Mock<DbSet<Category>>();
            mockCategories.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(dbCategories.Provider);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(dbCategories.Expression);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(dbCategories.ElementType);
            mockCategories.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(dbCategories.GetEnumerator());
        }

        private void SetupDbBrands()
        {
            SetupDbBrand();
            dbBrands = new List<Brand>
            {
                dbBrand1,
                dbBrand2
            }.AsQueryable();
        }

        private void SetupMockBrands()
        {
            mockBrands = new Mock<DbSet<Brand>>();
            mockBrands.As<IQueryable<Brand>>().Setup(m => m.Provider).Returns(dbBrands.Provider);
            mockBrands.As<IQueryable<Brand>>().Setup(m => m.Expression).Returns(dbBrands.Expression);
            mockBrands.As<IQueryable<Brand>>().Setup(m => m.ElementType).Returns(dbBrands.ElementType);
            mockBrands.As<IQueryable<Brand>>().Setup(m => m.GetEnumerator()).Returns(dbBrands.GetEnumerator());
        }

        private void SetupMockDbContext()
        {
            mockDbContext = new Mock<ProductDb>();
            if (mockProducts != null)
            {
                mockDbContext.Setup(m => m.Products).Returns(mockProducts.Object);
            }
            if (mockBrands != null)
            {
                mockDbContext.Setup(m => m.Brands).Returns(mockBrands.Object);
            }
            if (mockCategories != null)
            {
                mockDbContext.Setup(m => m.Categories).Returns(mockCategories.Object);
            }
        }

        private void DefaultSetup()
        {
            SetupMapper();
            SetupProductRepoModels();
            SetupDbProducts();
            SetupMockProducts();
            SetupDbBrands();
            SetupMockBrands();
            SetupDbCategories();
            SetupMockCategories();
            SetupMockDbContext();
            repo = new ProductRepository.ProductRepository(mockDbContext.Object, mapper);
        }

        [Fact]
        public async Task TwoNewProducts_OneAlreadyExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateProducts(productRepoModels);
            

            //Assert
            Assert.True(true == result);
            //check for add and save
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Exactly(1));
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Product product1 = dbProducts.FirstOrDefault(p => p.ProductId == productRepoModel1.ProductId);
            Assert.Equal(product1.Name, productRepoModel1.Name);
            Assert.Equal(product1.Description, productRepoModel1.Description);
            Assert.Equal(product1.Quantity, productRepoModel1.Quantity);
            Assert.Equal(product1.BrandId, productRepoModel1.BrandId);
            Assert.Equal(product1.CategoryId, productRepoModel1.CategoryId);
            Assert.Equal(product1.Price, productRepoModel1.Price);
        }

        [Fact]
        public async Task TwoNewProducts_NeitherAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].ProductId = 7;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Exactly(2));
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewProducts_BothAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[1].ProductId = 2;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Product product1 = dbProducts.FirstOrDefault(p => p.ProductId == productRepoModel1.ProductId);
            Assert.Equal(product1.Name, productRepoModel1.Name);
            Assert.Equal(product1.Description, productRepoModel1.Description);
            Assert.Equal(product1.Quantity, productRepoModel1.Quantity);
            Assert.Equal(product1.BrandId, productRepoModel1.BrandId);
            Assert.Equal(product1.CategoryId, productRepoModel1.CategoryId);
            Assert.Equal(product1.Price, productRepoModel1.Price);
            //check edit
            Product product2 = dbProducts.FirstOrDefault(p => p.ProductId == productRepoModel2.ProductId);
            Assert.Equal(product2.Name, productRepoModel2.Name);
            Assert.Equal(product2.Description, productRepoModel2.Description);
            Assert.Equal(product2.Quantity, productRepoModel2.Quantity);
            Assert.Equal(product2.BrandId, productRepoModel2.BrandId);
            Assert.Equal(product2.CategoryId, productRepoModel2.CategoryId);
            Assert.Equal(product2.Price, productRepoModel2.Price);
        }

        [Fact]
        public async Task TwoNewProducts_FirstDoesntExist_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].ProductId = 7;
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Once());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewProducts_FirsNull_SecondDoesntExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;
            productRepoModels[1].ProductId = 7;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Once());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewProducts_FirstExists_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].ProductId = 1;
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Product product1 = dbProducts.FirstOrDefault(p => p.ProductId == productRepoModel1.ProductId);
            Assert.Equal(product1.Name, productRepoModel1.Name);
            Assert.Equal(product1.Description, productRepoModel1.Description);
            Assert.Equal(product1.Quantity, productRepoModel1.Quantity);
            Assert.Equal(product1.BrandId, productRepoModel1.BrandId);
            Assert.Equal(product1.CategoryId, productRepoModel1.CategoryId);
            Assert.Equal(product1.Price, productRepoModel1.Price);
        }

        [Fact]
        public async Task TwoNewProducts_FirstNull_SecondExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;
            productRepoModels[1].ProductId = 2;

            //Act
            var result = await repo.UpdateProducts(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Product product2 = dbProducts.FirstOrDefault(p => p.ProductId == productRepoModel2.ProductId);
            Assert.Equal(product2.Name, productRepoModel2.Name);
            Assert.Equal(product2.Description, productRepoModel2.Description);
            Assert.Equal(product2.Quantity, productRepoModel2.Quantity);
            Assert.Equal(product2.BrandId, productRepoModel2.BrandId);
            Assert.Equal(product2.CategoryId, productRepoModel2.CategoryId);
            Assert.Equal(product2.Price, productRepoModel2.Price);
        }

        [Fact]
        public async Task NullProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateProducts(null);

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task EmptyProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateProducts(new List<ProductRepoModel>());

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task UpdateBrands_OneAlreadyExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[1].BrandId = 6;
            productRepoModels[1].Brand = "Brand 6";

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            //check for add and save
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Exactly(1));
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Brand brand1 = dbBrands.FirstOrDefault(b => b.BrandId == productRepoModel1.BrandId);
            Assert.Equal(brand1.BrandName, productRepoModel1.Brand);


        }

        [Fact]
        public async Task TwoNewBrands_NeitherAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].BrandId = 5;
            productRepoModels[0].Brand = "Brand 5";
            productRepoModels[1].BrandId = 6;
            productRepoModels[1].Brand = "Brand 6";

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Exactly(2));
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewBrands_BothAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Brand brand1 = dbBrands.FirstOrDefault(b => b.BrandId == productRepoModel1.BrandId);
            Assert.Equal(brand1.BrandName, productRepoModel1.Brand);
            //check edit
            Brand brand2 = dbBrands.FirstOrDefault(b => b.BrandId == productRepoModel2.BrandId);
            Assert.Equal(brand2.BrandName, productRepoModel2.Brand);
        }

        [Fact]
        public async Task TwoNewBrands_FirstDoesntExist_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].BrandId = 7;
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Exactly(1));
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewBrands_FirsNull_SecondDoesntExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;
            productRepoModels[1].BrandId = 7;

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Exactly(1));
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewBrands_FirstExists_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Brand brand1 = dbBrands.FirstOrDefault(b => b.BrandId == productRepoModel1.BrandId);
            Assert.Equal(brand1.BrandName, productRepoModel1.Brand);
        }

        [Fact]
        public async Task TwoNewBrands_FirstNull_SecondExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;

            //Act
            var result = await repo.UpdateBrands(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Brand brand2 = dbBrands.FirstOrDefault(b => b.BrandId == productRepoModel1.BrandId);
            Assert.Equal(brand2.BrandName, productRepoModel1.Brand);
        }

        [Fact]
        public async Task UpdateBrands_NullProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateBrands(null);

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task UpdateBrands_EmptyProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateBrands(new List<ProductRepoModel>());

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategories_OneAlreadyExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[1].CategoryId = 6;
            productRepoModels[1].Category = "Category 6";

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            //check for add and save
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never);
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Category category1 = dbCategories.FirstOrDefault(b => b.CategoryId == productRepoModel1.CategoryId);
            Assert.Equal(category1.CategoryName, productRepoModel1.Category);


        }

        [Fact]
        public async Task TwoNewCategories_NeitherAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].CategoryId = 5;
            productRepoModels[0].Category = "Category 5";
            productRepoModels[1].CategoryId = 6;
            productRepoModels[1].Category = "Category 6";

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Exactly(2));
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never);
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewCategories_BothAlreadyExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Category category1 = dbCategories.FirstOrDefault(b => b.CategoryId == productRepoModel1.CategoryId);
            Assert.Equal(category1.CategoryName, productRepoModel1.Category);
            //check edit
            Category category2 = dbCategories.FirstOrDefault(b => b.CategoryId == productRepoModel2.CategoryId);
            Assert.Equal(category2.CategoryName, productRepoModel2.Category);
        }

        [Fact]
        public async Task TwoNewCategories_FirstDoesntExist_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0].CategoryId = 7;
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never);
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewCategories_FirsNull_SecondDoesntExist_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;
            productRepoModels[1].CategoryId = 7;

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never);
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TwoNewCategories_FirstExists_SecondNull_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[1] = null;

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Category category1 = dbCategories.FirstOrDefault(b => b.CategoryId == productRepoModel1.CategoryId);
            Assert.Equal(category1.CategoryName, productRepoModel1.Category);
        }

        [Fact]
        public async Task TwoNewCategories_FirstNull_SecondExists_ShouldTrue()
        {
            //Arrange
            DefaultSetup();
            productRepoModels[0] = null;

            //Act
            var result = await repo.UpdateCategories(productRepoModels);

            //Assert
            Assert.True(true == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            //check edit
            Category category2 = dbCategories.FirstOrDefault(b => b.CategoryId == productRepoModel1.CategoryId);
            Assert.Equal(category2.CategoryName, productRepoModel1.Category);
        }

        [Fact]
        public async Task UpdateCategories_NullProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateCategories(null);

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task UpdateCategories_EmptyProducts_ShouldFalse()
        {
            //Arrange
            DefaultSetup();

            //Act
            var result = await repo.UpdateCategories(new List<ProductRepoModel>());

            //Assert
            Assert.True(false == result);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetProduct_ValidId_ShouldReturnProduct()
        {
            //Arrange
            DefaultSetup();

            //Act
            var product = await repo.GetProduct(1);

            //Assert
            Assert.NotNull(product);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            Assert.Equal(product.ProductId, dbProduct1.ProductId);
            Assert.Equal(product.Name, dbProduct1.Name);
            Assert.Equal(product.Description, dbProduct1.Description);
            Assert.Equal(product.Quantity, dbProduct1.Quantity);
            Assert.Equal(product.BrandId, dbProduct1.BrandId);
            Assert.Equal(product.Brand, dbBrand1.BrandName);
            Assert.Equal(product.CategoryId, dbProduct1.CategoryId);
            Assert.Equal(product.Category, dbCategory1.CategoryName);
            Assert.Equal(product.Price, dbProduct1.Price);
        }

        [Fact]
        public async Task GetProduct_DifferentValidId_ShouldReturnProduct()
        {
            //Arrange
            DefaultSetup();

            //Act
            var product = await repo.GetProduct(2);

            //Assert
            Assert.NotNull(product);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            Assert.Equal(product.ProductId, dbProduct2.ProductId);
            Assert.Equal(product.Name, dbProduct2.Name);
            Assert.Equal(product.Description, dbProduct2.Description);
            Assert.Equal(product.Quantity, dbProduct2.Quantity);
            Assert.Equal(product.BrandId, dbProduct2.BrandId);
            Assert.Equal(product.Brand, dbBrand2.BrandName);
            Assert.Equal(product.CategoryId, dbProduct2.CategoryId);
            Assert.Equal(product.Category, dbCategory2.CategoryName);
            Assert.Equal(product.Price, dbProduct2.Price);
        }

        [Fact]
        public async Task GetProduct_InvalidId_ShouldReturnNull()
        {
            //Arrange
            DefaultSetup();

            //Act
            var product = await repo.GetProduct(0);

            //Assert
            Assert.Null(product);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetProduct_ProductDoesntExists_ShouldReturnNull()
        {
            //Arrange
            DefaultSetup();

            //Act
            var product = await repo.GetProduct(99999);

            //Assert
            Assert.Null(product);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetProduct_NegativeId_ShouldReturnNull()
        {
            //Arrange
            DefaultSetup();

            //Act
            var product = await repo.GetProduct(-1);

            //Assert
            Assert.Null(product);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task GetProductInfo_ShouldReturnProductInfo()
        {
            //Arrange
            DefaultSetup();

            //Act
            var productInfo = await repo.GetProductInfo();

            //Assert
            Assert.NotNull(productInfo);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            for (int i = 0; i< productInfo.Brands.Count; i++)
            {
                Assert.Equal(dbBrands.Select(b => b.BrandName).ToList()[i], productInfo.Brands[i]);
            }
            for (int i = 0; i < productInfo.Categories.Count; i++)
            {
                Assert.Equal(dbCategories.Select(b => b.CategoryName).ToList()[i], productInfo.Categories[i]);
            }
        }

        [Fact]
        public async Task GetProductInfo_EmptyCategories_ShouldReturnProductInfo()
        {
            //Arrange
            SetupMapper();
            SetupProductRepoModels();
            SetupDbProducts();
            SetupMockProducts();
            SetupDbBrands();
            SetupMockBrands();
            dbCategories = new List<Category>().AsQueryable();
            SetupMockCategories();
            SetupMockDbContext();
            repo = new ProductRepository.ProductRepository(mockDbContext.Object, mapper);
            

            //Act
            var productInfo = await repo.GetProductInfo();

            //Assert
            Assert.NotNull(productInfo);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            for (int i = 0; i < productInfo.Brands.Count; i++)
            {
                Assert.Equal(dbBrands.Select(b => b.BrandName).ToList()[i], productInfo.Brands[i]);
            }
            Assert.True(productInfo.Categories.Count == 0);
        }

        [Fact]
        public async Task GetProductInfo_EmptyProducts_ShouldReturnProductInfo()
        {
            //Arrange
            SetupMapper();
            SetupProductRepoModels();
            SetupDbProducts();
            SetupMockProducts();
            dbBrands = new List<Brand>().AsQueryable();
            SetupMockBrands();
            SetupDbCategories();
            SetupMockCategories();
            SetupMockDbContext();
            repo = new ProductRepository.ProductRepository(mockDbContext.Object, mapper);


            //Act
            var productInfo = await repo.GetProductInfo();

            //Assert
            Assert.NotNull(productInfo);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            for (int i = 0; i < productInfo.Categories.Count; i++)
            {
                Assert.Equal(dbCategories.Select(b => b.CategoryName).ToList()[i], productInfo.Categories[i]);
            }
            Assert.True(productInfo.Brands.Count == 0);
        }

        [Fact]
        public async Task GetProductInfo_EmptyProductsAndCategories_ShouldReturnProductInfo()
        {
            //Arrange
            SetupMapper();
            SetupProductRepoModels();
            SetupDbProducts();
            SetupMockProducts();
            dbBrands = new List<Brand>().AsQueryable();
            SetupMockBrands();
            dbCategories = new List<Category>().AsQueryable();
            SetupMockCategories();
            SetupMockDbContext();
            repo = new ProductRepository.ProductRepository(mockDbContext.Object, mapper);


            //Act
            var productInfo = await repo.GetProductInfo();

            //Assert
            Assert.NotNull(productInfo);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            Assert.True(productInfo.Categories.Count == 0);
            Assert.True(productInfo.Brands.Count == 0);
        }

        [Fact]
        public async Task GetProducts_AllNull_ReturnsAllProducts()
        {
            //Arrange
            DefaultSetup();
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidBrandId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int brandId = 1;
            var expectedProducts = dbProducts.Where(p => p.BrandId == brandId).ToList();


            //Act
            var products = await repo.GetProducts(brandId, null, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidCategoryId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int categoryId = 1;
            var expectedProducts = dbProducts.Where(p => p.CategoryId == categoryId).ToList();


            //Act
            var products = await repo.GetProducts(null, categoryId, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidBrand_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string brand = "Brand 1";
            var expectedProducts = dbProducts.Where(p => p.BrandId == 1).ToList();


            //Act
            var products = await repo.GetProducts(null, null, brand, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidCategory_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string category = "Category 1";
            var expectedProducts = dbProducts.Where(p => p.CategoryId == 1).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, category, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidSearchName_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "uct 1";
            var expectedProducts = dbProducts.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString)).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidSearchDescription_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "ion 1";
            var expectedProducts = dbProducts.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString)).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidMinPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            double minPrice = 2;
            var expectedProducts = dbProducts.Where(p => p.Price >= minPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, minPrice, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidMaxPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            double maxPrice = 3;
            var expectedProducts = dbProducts.Where(p => p.Price <= maxPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, null, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        //Due to time restrainsts cannot test all possible combinations of nullable vars
        //A reasononable selection tested instead
        [Fact]
        public async Task GetProducts_ValidBrandIdAndCategoryId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int brandId = 1;
            int categoryId = 2;
            var expectedProducts = dbProducts.Where(p => p.BrandId <= brandId && p.CategoryId == categoryId).ToList();


            //Act
            var products = await repo.GetProducts(brandId, categoryId, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidBrandAndCategory_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string brand = "Brand 1";
            string category = "Category 1";
            var expectedProducts = dbProducts.Where(p => p.CategoryId == 1 && p.BrandId == 1).ToList();


            //Act
            var products = await repo.GetProducts(null, null, brand, category, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidSearchStringAndMinPriceAndMaxPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "oduct";
            double minPrice = 1;
            double maxPrice = 3;
            var expectedProducts = dbProducts.Where(p => p.Price <= maxPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, minPrice, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidBrandId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int brandId = 0;
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(brandId, null, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidCategoryId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int categoryId = 0;
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, categoryId, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidBrand_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string brand = "";
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, brand, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidCategory_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string category = "";
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, category, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidSearch_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "";
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidMinPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            double minPrice = 0;
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, minPrice, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidMaxPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            double maxPrice = 0;
            var expectedProducts = dbProducts.ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, null, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        //Due to time restrainsts cannot test all possible combinations of nullable vars
        //A reasononable selection tested instead
        [Fact]
        public async Task GetProducts_InalidBrandIdAndValidCategoryId_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            int brandId = 0;
            int categoryId = 2;
            var expectedProducts = dbProducts.Where(p => p.CategoryId == categoryId).ToList();


            //Act
            var products = await repo.GetProducts(brandId, categoryId, null, null, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidBrandAndValidCategory_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string brand = "";
            string category = "Category 1";
            var expectedProducts = dbProducts.Where(p => p.CategoryId == 1).ToList();


            //Act
            var products = await repo.GetProducts(null, null, brand, category, null, null, null);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_InvalidSearchStringAndValidMinPriceAndValidMaxPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "";
            double minPrice = 2;
            double maxPrice = 3;
            var expectedProducts = dbProducts.Where(p => p.Price <= maxPrice && p.Price >=minPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, minPrice, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_ValidalidSearchStringAndValidMinPriceAndInvalidMaxPrice_ReturnsProducts()
        {
            //Arrange
            DefaultSetup();
            string searchString = "uct";
            double minPrice = 1;
            double maxPrice = 0;
            var expectedProducts = dbProducts.Where(p => p.Name.Contains(searchString) ||p.Price <= maxPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, searchString, minPrice, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(expectedProducts.Count == products.Count);
            for (int i = 0; i < products.Count; i++)
            {
                Assert.Equal(expectedProducts[i].ProductId, products[i].ProductId);
                Assert.Equal(expectedProducts[i].Name, products[i].Name);
                Assert.Equal(expectedProducts[i].Description, products[i].Description);
                Assert.Equal(expectedProducts[i].Quantity, products[i].Quantity);
                Assert.Equal(expectedProducts[i].BrandId, products[i].BrandId);
                Assert.Equal(dbBrands.Where(b => b.BrandId == expectedProducts[i].BrandId).Select(b => b.BrandName).FirstOrDefault(), products[i].Brand);
                Assert.Equal(expectedProducts[i].CategoryId, products[i].CategoryId);
                Assert.Equal(dbCategories.Where(c => c.CategoryId == expectedProducts[i].CategoryId).Select(b => b.CategoryName).FirstOrDefault(), products[i].Category);
                Assert.Equal(expectedProducts[i].Price, products[i].Price);
            }
        }

        [Fact]
        public async Task GetProducts_TooRestrictiveValues_ReturnsEmptyProducts()
        {
            //Arrange
            DefaultSetup();
            double minPrice = 2.5;
            double maxPrice = 2.51;
            var expectedProducts = dbProducts.Where(p => p.Price <= maxPrice && p.Price >= minPrice).ToList();


            //Act
            var products = await repo.GetProducts(null, null, null, null, null, minPrice, maxPrice);

            //Assert
            Assert.NotNull(products);
            mockDbContext.Verify(m => m.Add(It.IsAny<Product>()), Times.Never());
            mockProducts.Verify(m => m.Remove(It.IsAny<Product>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Category>()), Times.Never());
            mockCategories.Verify(m => m.Remove(It.IsAny<Category>()), Times.Never());
            mockDbContext.Verify(m => m.Add(It.IsAny<Brand>()), Times.Never());
            mockBrands.Verify(m => m.Remove(It.IsAny<Brand>()), Times.Never());
            Assert.True(0 == products.Count);
        }
    }
}
