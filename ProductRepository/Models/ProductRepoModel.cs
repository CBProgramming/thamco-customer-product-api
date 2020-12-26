using System;
using System.Collections.Generic;
using System.Text;

namespace ProductRepository.Data
{
    public class ProductRepoModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public int BrandId { get; set; }

        public string Brand { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public double Price { get; set; }
    }
}
