﻿namespace AllupProjectMVC.Models
{
    public class WishlistProduct : BaseEntity
    {
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
