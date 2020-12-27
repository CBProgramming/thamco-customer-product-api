using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductRepository
{
    public interface IProductRepository
    {
        public Task<bool> UpdateProducts(IList<ProductRepoModel> products);

        public Task<bool> UpdateBrands(IList<ProductRepoModel> products);

        public Task<bool> UpdateCategories(IList<ProductRepoModel> products);

        public Task<ProductRepoModel> GetProduct(int productId);

        public Task<IList<ProductRepoModel>> GetProducts(int? brandId, int? categoryId, string? brand, 
            string? category, string? searchString, double? minPrice, double? maxPrice);

        public Task<ProductInfoRepoModel> GetProductInfo();
    }
}
