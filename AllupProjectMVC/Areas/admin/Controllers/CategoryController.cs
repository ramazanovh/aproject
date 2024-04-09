using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Models;
using AProjectMVC.Areas.admin.ViewModels.Category;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AProjectMVC.Areas.admin.Controllers
{
    public class CategoryController : MainController
    {
        private readonly ADbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        public CategoryController(ADbContext context,
                                    IWebHostEnvironment env,
                                    IMapper mapper,
                                    ICategoryService categoryService)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            CategoryVM category = await _categoryService.GetByIdAsync((int)id);

            if (category is null) return NotFound();

            return View(category);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM request)
        {


            CategoryVM existCategory = await _categoryService.GetByNameWithoutTrackingAsync(request.Name);

            if (existCategory is not null)
            {
                ModelState.AddModelError("Name", "This category already exists");

                return View(request);
            }


            if (!request.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File can be only image format");
                return View(request);
            }

            if (!request.Photo.CheckFilesize(200))
            {
                ModelState.AddModelError("Photo", "File size can be max 200 kb");
                return View(request);
            }



            await _categoryService.CreateAsync(request);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            Category dbCategory = await _context.Categories.AsNoTracking().Where(m => m.Id == id).FirstOrDefaultAsync();

            if (dbCategory is null) return NotFound();


            return View(new CategoryUpdateVM()
            {
                Name = dbCategory.Name,
                Image = dbCategory.Image,

            });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, CategoryUpdateVM request)
        {
            if (id is null) return BadRequest();



            Category dbCategory = await _context.Categories.Where(m => m.Id == id)
                                            .FirstOrDefaultAsync();

            if (dbCategory is null) return NotFound();


            request.Image = dbCategory.Image;

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            CategoryVM existCategory = await _categoryService.GetByNameWithoutTrackingAsync(request.Name);


            if (request.Photo != null)
            {


                if (!request.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photos", "File can only be in image format");
                    return View(request);

                }

                if (!request.Photo.CheckFilesize(200))
                {
                    ModelState.AddModelError("Photos", "File size can be max 200 kb");
                    return View(request);
                }

            }
            else
            {
                return RedirectToAction(nameof(Index));
            }



            if (existCategory is not null)
            {
                if (existCategory.Id == request.Id)
                {

                     await _categoryService.UpdateAsync(request);

                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("Name", "This category already exists");
                return View(request);
            }

            await _categoryService.UpdateAsync(request);

            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {

            await _categoryService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));

        }


    }
}
