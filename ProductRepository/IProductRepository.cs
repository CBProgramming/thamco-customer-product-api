using ProductRepository.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductRepository
{
    public interface IProductRepository
    {
        public Task<bool> EditProduct(ProductEFModel product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> NewProduct(ProductEFModel product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductEFModel> GetProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ProductEFModel>> GetProducts(string? searchTerm)
        {
            throw new NotImplementedException();
        }
    }
}
