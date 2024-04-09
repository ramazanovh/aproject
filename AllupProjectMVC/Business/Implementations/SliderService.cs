using AllupProjectMVC.Areas.admin.ViewModels.Slider;
using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace AllupProjectMVC.Business.Implementations
{
    public class SliderService : ISliderService
    {
        private readonly ADbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public SliderService(ADbContext context,
                              IMapper mapper,
                              IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<List<SliderVM>> GetAllAsync()
        {
            return _mapper.Map<List<SliderVM>>(await _context.Sliders.ToListAsync());

        }

        public async Task<SliderVM> GetByIdAsync(int id)
        {
            return _mapper.Map<SliderVM>(await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id));
        }

        public async Task CreateAsync(SliderCreateVM slider)
        {
            string fileName = $"{Guid.NewGuid()}-{slider.Photo.FileName}";

            string path = _env.GetFilePath("uploads/sliders", fileName);

            var data = _mapper.Map<Slider>(slider);

            data.Image = fileName;

            await _context.AddAsync(data);

            await _context.SaveChangesAsync();

            await slider.Photo.SaveFileAsync(path);

        }

        public async Task UpdateAsync(SliderUpdateVM slider)
        {
            Slider dbSlider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == slider.Id);
            if (slider.Photo != null)
            {

                string oldPath = _env.GetFilePath("uploads/sliders", slider.Image);

                string fileName = $"{Guid.NewGuid()}-{slider.Photo.FileName}";

                string newPath = _env.GetFilePath("uploads/sliders", fileName);
                dbSlider.Image = fileName;

                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }

                await slider.Photo.SaveFileAsync(newPath);

            }
            dbSlider.Title = slider.Title;
            dbSlider.Title2 = slider.Title2;
            dbSlider.Description = slider.Description;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Slider slider = await _context.Sliders.Where(m => m.Id == id).FirstOrDefaultAsync();
            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();

            string path = _env.GetFilePath("uploads/sliders", slider.Image);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
