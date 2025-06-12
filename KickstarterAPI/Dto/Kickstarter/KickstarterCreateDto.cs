using System.ComponentModel.DataAnnotations;

namespace KickstarterAPI.Dto.Kickstarter;

public class KickstarterCreateDto
{
    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(100, ErrorMessage = "Project name can be at most 100 characters long.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category can be at most 50 characters long.")]
    public string Category { get; set; }

    [Required(ErrorMessage = "Subcategory is required.")]
    [StringLength(50, ErrorMessage = "Subcategory can be at most 50 characters long.")]
    public string Subcategory { get; set; }

    [Required(ErrorMessage = "Country code is required.")]
    [StringLength(50, ErrorMessage = "Country code can be at most 50 characters long.")]
    public string Country { get; set; }

    public DateTime Launched { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Deadline is required.")]
    public DateTime Deadline { get; set; }

    [Required(ErrorMessage = "Funding goal is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Funding goal must be greater than 0.")]
    public int Goal { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Pledged amount cannot be negative.")]
    public int Pledged { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Number of backers must be 0 or greater.")]
    public int Backers { get; set; } = 0;

    public string State { get; set; } = "Live";
}