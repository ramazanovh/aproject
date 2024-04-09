﻿namespace AllupProjectMVC.ViewModels.Basket
{
    public class BasketDetailVm
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
