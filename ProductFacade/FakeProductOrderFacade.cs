using ProductOrderFacade.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductOrderFacade
{
    public class FakeProductOrderFacade : IProductOrderFacade
    {
        bool Succeeds = true;
        IList<ProductUpdateDto> Products;

        public async Task<bool> UpdateProducts(IList<ProductUpdateDto> products)
        {
            Products = products;
            return Succeeds;
        }
    }
}
