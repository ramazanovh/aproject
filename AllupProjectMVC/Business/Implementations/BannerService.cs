using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Data;
using AllupProjectMVC.Extension;
using AllupProjectMVC.Models;
using AProjectMVC.Areas.admin.ViewModels.Banner;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AllupProjectMVC.Business.Implementations
{
    public class BannerService : IBannerService
    {
        private readonly ADbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public BannerService(ADbContext context,
                             IMapper mapper,
                             IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<BannerVM> GetAllAsync()
        {
            return _mapper.Map<BannerVM>(await _context.Banners.FirstOrDefaultAsync());
        }

        public async Task<BannerVM> GetByIdAsync(int id)
        {
            return _mapper.Map<BannerVM>(await _context.Banners.FirstOrDefaultAsync(m => m.Id == id));

        }

        public async Task UpdateAsync(BannerUpdateVM request)
        {
            string oldPath = _env.GetFilePath("uploads/banners", request.Image);

            string fileName = $"{Guid.NewGuid()} - {request.Photo.FileName}";

            string newPath = _env.GetFilePath("uploads/banners", fileName);

            Banner dbBanner = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.Id);


            _mapper.Map(request, dbBanner);

            dbBanner.Image = fileName;

            _context.Banners.Update(dbBanner);
            await _context.SaveChangesAsync();



            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
            }

            await request.Photo.SaveFileAsync(newPath);
        }

        


    }
}
