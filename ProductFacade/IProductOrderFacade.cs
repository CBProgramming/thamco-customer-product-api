using ProductOrderFacade.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOrderFacade
{
    public interface IProductOrderFacade
    {
        public Task<bool> UpdateProducts(IList<ProductUpdateDto> products);
    }
}
