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
        }
    }
}