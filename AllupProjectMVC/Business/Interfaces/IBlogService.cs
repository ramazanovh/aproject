using AllupProjectMVC.Areas.admin.ViewModels.Blog;
using AllupProjectMVC.Models;
using AProjectMVC.Areas.admin.ViewModels.Blog;
using System.Linq.Expressions;

namespace AllupProjectMVC.Business.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogVM>> GetAllAsync();
        Task<BlogVM> GetByIdAsync(int id);
        Task CreateAsync(BlogCreateVM request);
        Task DeleteAsync(int id);
        Task UpdateAsync(BlogUpdateVM request);
        Task CreateAsync(BlogCreateVM request);
    }
}
