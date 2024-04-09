using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllupProjectMVC.Models;

public class Slider : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string Title { get; set; }
    [Required]
    [StringLength(20)]
    public string Title2 { get; set; }
    [Required]
    [StringLength(100)]
    public string Description { get; set; }
    public string Image { get; set; }
    public object ImageFile { get; internal set; }
    public object ImageUrl { get; internal set; }
}
