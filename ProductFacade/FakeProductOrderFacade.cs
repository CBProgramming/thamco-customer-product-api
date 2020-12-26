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
        public async Task<bool> UpdateProducts(IList<ProductUpdateDto> products)
        {
            return Succeeds;
        }
    }
}
