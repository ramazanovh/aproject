using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Helpers;
using AllupProjectMVC.Models;
using AProjectMVC.Areas.admin.ViewModels.Product;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AProjectMVC.Areas.admin.Controllers
{
    [Area("Admin")]
    public class ProductController : MainController
    {
        private readonly IProductService _productService;
        private readonly ADbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService,
                                 ADbContext context,
                                 IWebHostEnvironment env,
                                 ICategoryService categoryService,
                                 IMapper mapper)
        {
            _productService = productService;
            _context = context;
            _env = env;
            _categoryService = categoryService;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int take = 5)
        {
            List<ProductVM> dbPaginatedDatas = await _productService.GetPaginatedDatasAsync(page, take);

            int pageCount = await GetPageCountAsync(take);

            Paginate<ProductVM> paginatedDatas = new(dbPaginatedDatas, page, pageCount);

            return View(paginatedDatas);
        }

        private async Task<int> GetPageCountAsync(int take)
        {
            int productCount = await _productService.GetCountAsync();
            return (int)Math.Ceiling((decimal)(productCount) / take);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);


            if (product is null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }
        private async Task<SelectList> GetCategoriesAsync()
        {
            return new SelectList(await _categoryService.getAllCategoriesAsync(), "Id", "Title");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM request)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            foreach (var photo in request.Photos)
            {

                if (!photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photos", "File can be only image format");
                    return View(request);
                }

                if (!photo.CheckFilesize(200))
                {
                    ModelState.AddModelError("Photos", "File size can be max 200 kb");
                    return View(request);
                }
            }

            List<ProductImage> newImages = new();

            foreach (var photo in request.Photos)
            {
                string fileName = $"{Guid.NewGuid()}-{photo.FileName}";

                string path = _env.GetFilePath("uploads/product", fileName);

                await photo.SaveFileAsync(path);

                newImages.Add(new ProductImage { Image = fileName });
            }

            newImages.FirstOrDefault().IsPoster = true;

            await _context.ProductImages.AddRangeAsync(newImages);

            await _context.Products.AddAsync(new Product
            {
                Title = request.Name,
                Description = request.Description,
                Price = (int)request.Price,
                CategoryId = request.CategoryId,
                Images = newImages
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            ProductImage image = await _context.ProductImages.Where(m => m.Id == id).FirstOrDefaultAsync();
            _context.ProductImages.Remove(image);

            await _context.SaveChangesAsync();

            string path = _env.GetFilePath("uploads/product", image.Image);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            return View(new ProductUpdateVM
            {
                Id = product.Id,
                Name = product.Title,
                Description = product.Description,
                CategoryId = (int)product.CategoryId,
                Price = product.Price,
                Images = product.Images.ToList(),
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductUpdateVM request)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (id is null) return BadRequest();

            Product product = await _productService.GetByIdAsync((int)id);

            if (product is null) return NotFound();

            request.Images = product.Images.ToList();

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            List<ProductImage> newImages = new();

            if (request.Photos != null)
            {
                foreach (var photo in request.Photos)
                {

                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photos", "File can be only image format");
                        return View(request);
                    }

                    if (!photo.CheckFilesize(200))
                    {
                        ModelState.AddModelError("Photos", "File size can be max 200 kb");
                        return View(request);
                    }
                }

                foreach (var photo in request.Photos)
                {
                    string fileName = $"{Guid.NewGuid()}-{photo.FileName}";

                    string path = _env.GetFilePath("uploads/product", fileName);

                    await photo.SaveFileAsync(path);

                    newImages.Add(new ProductImage { Image = fileName });
                }

                await _context.ProductImages.AddRangeAsync(newImages);
            }

            newImages.AddRange(request.Images);

            product.Title = request.Name;
            product.Description = request.Description;
            product.Price = (int)request.Price;
            product.CategoryId = request.CategoryId;
            product.Images = newImages;


            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {

            await _productService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));

        }
    }
}
