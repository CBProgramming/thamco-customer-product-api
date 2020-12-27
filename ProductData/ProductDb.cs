using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductData
{
    public class ProductDb : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public ProductDb(DbContextOptions<ProductDb> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("products");

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Brand>()
                .Property(b => b.BrandId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Product>()
                .HasData(
                    new Product
                    {
                        ProductId = 1,
                        Name = "Product 1",
                        Description = "Desc 1",
                        Quantity = 10,
                        BrandId = 1,
                        CategoryId = 1,
                        Price = 1.99
                    },
                    new Product
                    {
                        ProductId = 2,
                        Name = "Product 2",
                        Description = "Desc 2",
                        Quantity = 20,
                        BrandId = 2,
                        CategoryId = 2,
                        Price = 2.99
                    },
                    new Product
                    {
                        ProductId = 3,
                        Name = "Product 3",
                        Description = "Desc 3",
                        Quantity = 30,
                        BrandId = 1,
                        CategoryId = 2,
                        Price = 3.99
                    },
                    new Product
                    {
                        ProductId = 4,
                        Name = "Product 4",
                        Description = "Desc 4",
                        Quantity = 40,
                        BrandId = 2,
                        CategoryId = 1,
                        Price = 4.99
                    });

            modelBuilder.Entity<Brand>()
                .HasData(
                    new Brand
                    {
                        BrandId = 1,
                        BrandName = "Brand 1"
                    },
                    new Brand
                    {
                        BrandId = 2,
                        BrandName = "Brand 2"
                    });

            modelBuilder.Entity<Category>()
                .HasData(
                    new Category
                    {
                        CategoryId = 1,
                        CategoryName = "Category 1"
                    },
                    new Category
                    {
                        CategoryId = 2,
                        CategoryName = "Category 2"
                    });
        }
    }
}