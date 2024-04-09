﻿using AllupProjectMVC.Areas.admin.ViewModels.Blog;
using AllupProjectMVC.Areas.admin.ViewModels.Category;
using AllupProjectMVC.Areas.admin.ViewModels.Product;
using AllupProjectMVC.Areas.admin.ViewModels.Slider;
using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Models;
using AllupProjectMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AllupProjectMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ADbContext _context;
        private readonly ISliderService _sliderService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;
        private readonly IWishlistService _wishlistService;
        public HomeController(ADbContext context,
                              ISliderService sliderService,
                              ICategoryService categoryService,
                              IProductService productService,
                              IBlogService blogService,
                              IWishlistService wishlistService)
        {
            _context = context;
            _sliderService = sliderService;
            _categoryService = categoryService;
            _productService = productService;
            _blogService = blogService;
            _wishlistService = wishlistService;

        }
        public async Task<IActionResult> Index()
        {
            List<SliderVM> sliders = await _sliderService.GetAllAsync();
            List<ProductVM> products = await _productService.GetAllAsync();

            List<CategoryVM> categories = await _categoryService.GetAllAsync();
            List<BlogVM> blogs = await _blogService.GetAllAsync();
            HomeViewModel homeVM = new()
            {
                Sliders = sliders,
                Categories = categories,
                Products = products,
                Blogs = blogs,
            };



            return View(homeVM);
        }

        public async Task<IActionResult> GetProductDatasModal(int? id)
        {
            if (id is null) return BadRequest();
            var dbProduct = await _productService.GetProductDatasModalAsync((int)id);
            if (dbProduct is null) return NotFound();
            var categoryName = dbProduct.Category.Name;
            var mainImage = dbProduct.Images.Where(p => p.IsMain).FirstOrDefault().Image;

            ModalVM model = new()
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Price = dbProduct.Price,
                Description = dbProduct.Description,
                CategoryName = categoryName,
                Image = mainImage

            };
            return Ok(model);

        }


        public bool CheckIfProductInWishlist(int wishlistId, int productId)
        {
            return _context.WishlistProducts
                .Any(m => m.WishlistId == wishlistId && m.ProductId == productId);
        }




    }
}
