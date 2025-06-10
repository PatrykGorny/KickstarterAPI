namespace KickstarterAPI.Dto.Kickstarter;

public class KickstarterProjectDto
{
    public long ID { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Country { get; set; }
    public DateTime Launched { get; set; }
    public DateTime Deadline { get; set; }
    public decimal Goal { get; set; }
    public decimal Pledged { get; set; }
    public int Backers { get; set; }
    public string State { get; set; }
}