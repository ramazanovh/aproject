using AllupProjectMVC.Areas.admin.ViewModels.Blog;
using AllupProjectMVC.Areas.admin.ViewModels.Category;
using AllupProjectMVC.Areas.admin.ViewModels.Product;
using AllupProjectMVC.Areas.admin.ViewModels.Slider;
using AllupProjectMVC.Models;
using System;

namespace AllupProjectMVC.ViewModels;

public class HomeViewModel
{
    public List<SliderVM> Sliders { get; set; }
    public List<CategoryVM> Categories { get; set; }
    public List<ProductVM> Products { get; set; }
    public List<BlogVM> Blogs { get; set; }
    public bool IsInWishlist { get; set; }
}
