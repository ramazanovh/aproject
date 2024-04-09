using AllupProjectMVC.Data;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AProjectMVC.Areas.admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly ADbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(ADbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
            => View(await _context.Sliders.ToListAsync());

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();
            if (slider.ImageFile is not null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Content type must be png or jpeg!");
                    return View();
                }
                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Size must be lower than 2mb!");
                    return View();
                }
                slider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image is required!");
                return View();
            }
            slider.CreatedDate = DateTime.UtcNow;
            slider.ModifiedDate = DateTime.UtcNow;
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return Redirect(Url.Action("Index", "Slider"));

        }

        public async Task<IActionResult> Update(int Id)
        {
            var existSlider = await _context.Sliders.FindAsync(Id);
            if (existSlider is null) throw new Exception();
            return View(existSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Slider slider)
        {
            if (!ModelState.IsValid) return View();
            var existSlider = await _context.Sliders.FindAsync(slider.Id);
            if (existSlider is null) throw new Exception();

            if (slider.ImageFile is not null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Content type must be png or jpeg");
                    return View();
                }
                if (slider.ImageFile.Length <= 2097152)
                {
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Size must be lower tahn 2mb!");
                    return View();
                }

                //FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", existSlider.ImageUrl);
                //existSlider.ImageUrl = slider.ImageFile.SaveFile(_env.WebRootPath, "uploads/sliders");
            }
            existSlider.Title = slider.Title;
            existSlider.Title2 = slider.Title2;
            existSlider.Description = slider.Description;
            //existSlider.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int Id)
        {
            var existSlider = await _context.Sliders.FindAsync(Id);
            if (existSlider == null) return NotFound();

            FileManager.DeleteFile(_env.WebRootPath, "uploads/sliders", existSlider.ImageUrl);

            _context.Remove(existSlider);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
