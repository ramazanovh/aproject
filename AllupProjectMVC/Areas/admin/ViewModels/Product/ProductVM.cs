﻿using AllupProjectMVC.Models;

namespace AProjectMVC.Areas.admin.ViewModels.Product
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<ProductImage> Images { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
