using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductRepository
{
    public class FakeProductRepository : IProductRepository
    {
        public ProductRepoModel ProductRepoModel {get; set;}

        public List<String> Brands { get; set; }

        public List<String> Categories { get; set; }

        public Task<ProductRepoModel> GetProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductInfoRepoModel> GetProductInfo()
        {
            return new ProductInfoRepoModel
            {
                Brands = Brands,
                Categories = Categories
            };
        }

        public Task<IList<ProductRepoModel>> GetProducts(int? brandId, int? categoryId, string brand, string category, string searchString, double? minPrice, double? maxPrice)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateBrands(IList<ProductRepoModel> products)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategories(IList<ProductRepoModel> products)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProducts(IList<ProductRepoModel> products)
        {
            throw new NotImplementedException();
        }
    }
}
