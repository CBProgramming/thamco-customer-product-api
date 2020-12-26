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

        public async Task<bool> ProductExists(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }

        public async Task<bool> NewProduct(ProductRepoModel productModel)
        {
            if (productModel != null)
            {
                try
                {
                    if (await UpdateCategories(productModel.CategoryId, productModel.Category)
                        && await UpdateBrands(productModel.BrandId, productModel.Brand))
                    {
                        var product = _mapper.Map<Product>(productModel);
                        _context.Add(product);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
            return false;
        }

        public async Task<bool> EditProduct(ProductRepoModel productModel)
        {
            if (productModel != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productModel.ProductId);
                if (product != null)
                {
                    try
                    {
                        if (await UpdateCategories(productModel.CategoryId, productModel.Category)
                        && await UpdateBrands(productModel.BrandId, productModel.Brand))
                        {
                            product.Name = productModel.Name;
                            product.Description = productModel.Description;
                            product.Price = productModel.Price;
                            product.Quantity += productModel.Quantity;
                            product.BrandId = productModel.BrandId;
                            product.CategoryId = productModel.CategoryId;
                            await _context.SaveChangesAsync();
                            return true;
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                    }
                }
            }
            return false;
        }

        private async Task<bool> UpdateBrands(int brandId, string brand)
        {
            try
            {
                var dbBrand = _context.Brands.SingleOrDefault(b => b.BrandId == brandId);
                if (dbBrand == null)
                {
                    _context.Add(new Brand
                    {
                        BrandId = brandId,
                        BrandName = brand
                    });
                }
                else if (dbBrand.BrandName == brand)
                {
                    return true;
                }
                else
                {
                    dbBrand.BrandName = brand;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
            }
            return false;
        }

        private async Task<bool> UpdateCategories(int categoryId, string category)
        {
            try
            {
                var dbCategory = _context.Categories.SingleOrDefault(b => b.CategoryId == categoryId);
                if (dbCategory == null)
                {
                    _context.Add(new Category
                    {
                        CategoryId = categoryId,
                        CategoryName = category
                    });
                }
                else if (dbCategory.CategoryName == category)
                {
                    return true;
                }
                else
                {
                    dbCategory.CategoryName = category;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
            }
            return false;
        }

        public Task<ProductRepoModel> GetProduct(int productId)
        {
            throw new NotImplementedException();
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

        public async Task<IList<ProductRepoModel>> GetProducts(int? productId, int? brandId, int? categoryId, string? brand, 
            string? category, string? searchString, double? minPrice, double? maxPrice)
        {
            IQueryable<Product> products = _context.Products;
            if (productId != null && productId > 0)
            {
                products = products.Where(p => p.ProductId == productId);
            }
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
            return _mapper.Map<List<ProductRepoModel>>((from product in products select product).ToList());
        }
    }
}
