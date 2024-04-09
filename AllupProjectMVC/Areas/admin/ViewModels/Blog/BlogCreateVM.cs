using System.ComponentModel.DataAnnotations;

namespace AProjectMVC.Areas.admin.ViewModels.Blog
{
    public class BlogCreateVM
    {
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
