using AllupProjectMVC.Business.Implementations;
using AllupProjectMVC.Business.Interfaces;
using AllupProjectMVC.Models;

namespace AllupProjectMVC
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services) 
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
