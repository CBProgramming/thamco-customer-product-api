using ProductRepository.Data;
using ProductRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductRepository
{
    public class FakeProductRepository : IProductRepository
    {
        public bool RepoSucceeds = true;

        public ProductRepoModel ProductRepoModel {get; set;}

        public IList<ProductRepoModel> RepoProducts { get; set; }

        public List<String> Brands { get; set; }

        public List<String> Categories { get; set; }

        public async Task<ProductRepoModel> GetProduct(int productId)
        {
            if (RepoSucceeds && productId == ProductRepoModel.ProductId)
            {
                return ProductRepoModel;
            }
            else return null;
        }

        public async Task<ProductInfoRepoModel> GetProductInfo()
        {
            if (RepoSucceeds)
            {
                return new ProductInfoRepoModel
                {
                    Brands = Brands,
                    Categories = Categories
                };
            }
            return new ProductInfoRepoModel
            {
                Brands = new List<string>(),
                Categories = new List<string>()
            }; ;
        }

        public async Task<IList<ProductRepoModel>> GetProducts(int? brandId, int? categoryId, 
            string brand, string category, string searchString, double? minPrice, double? maxPrice)
        {
            if(!RepoSucceeds)
            {
                return new List<ProductRepoModel>();
            }
            IList<ProductRepoModel> result = RepoProducts;
            IEnumerable<ProductRepoModel> toRemove;
            if (brandId != null && brandId > 0)
            {
                toRemove = result.Where(p => p.BrandId != brandId).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (categoryId != null && categoryId > 0)
            {
                toRemove = result.Where(p => p.CategoryId != categoryId).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (minPrice != null && minPrice > 0)
            {
                toRemove = result.Where(p => p.Price < minPrice).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (maxPrice != null && maxPrice > 0)
            {
                toRemove = result.Where(p => p.Price > maxPrice).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                toRemove = result.Where(p => !p.Name.Contains(searchString) && !p.Description.Contains(searchString)).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (!string.IsNullOrEmpty(brand))
            {
                toRemove = result.Where(p => p.Brand != brand).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            if (!string.IsNullOrEmpty(category))
            {
                toRemove = result.Where(p => p.Category != category).ToList();
                if (toRemove.Count() > 0)
                {
                    foreach (ProductRepoModel product in toRemove)
                    {
                        RepoProducts.Remove(product);
                    }
                }
            }
            return result;
        }

        public async Task<bool> UpdateBrands(IList<ProductRepoModel> products)
        {
            if (RepoSucceeds)
            {
                List<string> brands = new List<string>();
                foreach (ProductRepoModel product in products)
                {
                    if (!brands.Contains(product.Brand))
                    {
                        brands.Add(product.Brand);
                    }
                }
                Brands = brands;
            }
            return RepoSucceeds;
        }

        public async Task<bool> UpdateCategories(IList<ProductRepoModel> products)
        {
            if (RepoSucceeds)
            {
                List<string> categories = new List<string>();
                foreach (ProductRepoModel product in products)
                {
                    if (!categories.Contains(product.Category))
                    {
                        categories.Add(product.Category);
                    }
                }
                Categories = categories;
            }
            return RepoSucceeds;
        }

        public async Task<bool> UpdateProducts(IList<ProductRepoModel> products)
        {
            if (RepoSucceeds)
            {
                RepoProducts = products;
            }
            return RepoSucceeds;
        }
    }
}
