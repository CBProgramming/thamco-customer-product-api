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
        public Task<bool> EditProduct(ProductRepoModel product);

        public Task<bool> NewProduct(ProductRepoModel product);

        public Task<bool> ProductExists(int id);

        public Task<ProductRepoModel> GetProduct(int productId);

        public Task<IList<ProductRepoModel>> GetProducts(int? productId, int? brandId, int? categoryId, string? brand, 
            string? category, string? searchString, double? minPrice, double? maxPrice);

        public Task<ProductInfoRepoModel> GetProductInfo();
    }
}
