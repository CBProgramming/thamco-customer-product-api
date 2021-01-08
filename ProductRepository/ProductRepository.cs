using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductData;
using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDb _context;
        private readonly IMapper _mapper;

        public ProductRepository(ProductDb context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> UpdateProducts(IList<ProductRepoModel> products)
        {
            if (products == null || products.Count == 0)
            {
                return false;
            }
            try
            {
                foreach (ProductRepoModel productModel in products)
                {
                    if(productModel != null)
                    {
                        var product = _context.Products.FirstOrDefault(p => p.ProductId == productModel.ProductId);
                        if (product == null)
                        {
                            product = _mapper.Map<Product>(productModel);
                            _context.Add(product);
                        }
                        else
                        {
                            product.Name = productModel.Name;
                            product.Description = productModel.Description;
                            product.Price = productModel.Price;
                            product.Quantity = productModel.Quantity;
                            product.BrandId = productModel.BrandId;
                            product.CategoryId = productModel.CategoryId;
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBrands(IList<ProductRepoModel> products)
        {
            if (products == null || products.Count == 0)
            {
                return false;
            }
            try
            {
                foreach (ProductRepoModel productModel in products)
                {
                    if (productModel != null)
                    {
                        var dbBrand = _context.Brands.SingleOrDefault(b => b.BrandId == productModel.BrandId);
                        if (dbBrand == null)
                        {
                            _context.Add(new Brand
                            {
                                BrandId = productModel.BrandId,
                                BrandName = productModel.Brand
                            });
                        }
                        else if (!string.IsNullOrEmpty(productModel.Brand) && dbBrand.BrandName != productModel.Brand)
                        {
                            dbBrand.BrandName = productModel.Brand;
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCategories(IList<ProductRepoModel> products)
        {
            if (products == null || products.Count == 0)
            {
                return false;
            }
            try
            {
                foreach (ProductRepoModel productModel in products)
                {
                    if (productModel != null)
                    {
                        var dbCategory = _context.Categories.SingleOrDefault(b => b.CategoryId == productModel.CategoryId);
                        if (dbCategory == null)
                        {
                            _context.Add(new Category
                            {
                                CategoryId = productModel.CategoryId,
                                CategoryName = productModel.Category
                            });
                        }
                        else if (!string.IsNullOrEmpty(productModel.Category) && dbCategory.CategoryName != productModel.Category)
                        {
                            dbCategory.CategoryName = productModel.Category;
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductRepoModel> GetProduct(int productId)
        {
            IQueryable<Product> product = _context.Products
                .Where(p => p.ProductId == productId);
            var result = JoinBrandsAndCategories(product).FirstOrDefault();
            return result;
        }

        private IQueryable<ProductRepoModel> JoinBrandsAndCategories(IQueryable<Product> query)
        {
            return query.Join(_context.Brands,
                p => p.BrandId,
                b => b.BrandId,
                (midProduct, brand) => new { midProduct, brand })
                .Join(_context.Categories,
                p => p.midProduct.CategoryId,
                c => c.CategoryId,
                (product, category) => new ProductRepoModel
                {
                    ProductId = product.midProduct.ProductId,
                    Name = product.midProduct.Name,
                    Description = product.midProduct.Description,
                    Quantity = product.midProduct.Quantity,
                    BrandId = product.midProduct.BrandId,
                    Brand = product.brand.BrandName,
                    CategoryId = product.midProduct.CategoryId,
                    Category = category.CategoryName,
                    Price = product.midProduct.Price
                });
        }

        public async Task<ProductInfoRepoModel> GetProductInfo()
        {
            return new ProductInfoRepoModel
            {
                Brands = await GetBrandNames(),
                Categories = await GetCategoryNames()
            };
        }

        private async Task<List<String>> GetBrandNames()
        {
            return _context.Brands.Select(b => b.BrandName).ToList();
        }

        private async Task<List<String>> GetCategoryNames()
        {
            return _context.Categories.Select(b => b.CategoryName).ToList();
        }

        public async Task<IList<ProductRepoModel>> GetProducts(int? brandId, int? categoryId, string? brand, 
            string? category, string? searchString, double? minPrice, double? maxPrice)
        {
            IQueryable<Product> products = _context.Products;
            if (brandId != null && brandId > 0)
            {
                products = products.Where(p => p.BrandId == brandId);
            }
            if (categoryId != null && categoryId > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId);
            }
            if (minPrice != null && minPrice > 0)
            {
                products = products.Where(p => p.Price >= minPrice);
            }
            if (maxPrice != null && maxPrice > 0)
            {
                products = products.Where(p => p.Price <= maxPrice);
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(brand))
            {
                products = products.Where(p => p.BrandId == _context.Brands
                .FirstOrDefault(b => b.BrandName == brand).BrandId);
            }
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.CategoryId == _context.Categories
                .FirstOrDefault(c => c.CategoryName == category).CategoryId);
            }
            var result = JoinBrandsAndCategories(products).ToList();
            return result;
        }
    }
}
