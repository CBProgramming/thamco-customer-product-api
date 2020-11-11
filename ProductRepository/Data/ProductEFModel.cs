using System;
using System.Collections.Generic;
using System.Text;

namespace ProductRepository.Data
{
    public class ProductEFModel
    {
        public int productId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }
    }
}
