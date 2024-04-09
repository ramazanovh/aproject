using AProjectMVC.Areas.admin.ViewModels.Banner;

namespace AllupProjectMVC.Business.Interfaces
{
    public interface IBannerService
    {
        Task<BannerVM> GetAllAsync();
        Task<BannerVM> GetByIdAsync(int id);
        Task UpdateAsync(BannerUpdateVM request);   
    }
}
