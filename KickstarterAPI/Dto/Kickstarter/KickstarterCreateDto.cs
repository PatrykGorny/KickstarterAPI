using System.ComponentModel.DataAnnotations;

namespace KickstarterAPI.Dto.Kickstarter;

public class KickstarterCreateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public string Subcategory { get; set; }

    [Required]
    public string Country { get; set; }

    public DateTime Launched { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime Deadline { get; set; }

    [Range(1, double.MaxValue)]
    public decimal Goal { get; set; }

    public decimal Pledged { get; set; } = 0;

    public int Backers { get; set; } = 0;

    public string State { get; set; } = "Live";
}