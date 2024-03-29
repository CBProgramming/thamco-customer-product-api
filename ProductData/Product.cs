﻿using System;

namespace ProductData
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public int BrandId { get; set; }

        public int CategoryId { get; set; }

        public double Price { get; set; }
    }
}
