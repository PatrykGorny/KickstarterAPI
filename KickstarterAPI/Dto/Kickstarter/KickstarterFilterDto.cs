namespace KickstarterAPI.Dto.Kickstarter;

public class KickstarterFilterDto
{
    public long? ID { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Subcategory { get; set; }
    public string? Country { get; set; }
    
    public DateTime? LaunchedFrom { get; set; }
    public DateTime? LaunchedTo { get; set; }
    public DateTime? DeadlineFrom { get; set; }
    public DateTime? DeadlineTo { get; set; }
    
    public int? GoalMin { get; set; }
    public int? GoalMax { get; set; }
    public int? PledgedMin { get; set; }
    public int? PledgedMax { get; set; }
    public int? BackersMin { get; set; }
    public int? BackersMax { get; set; }
    
    public string? State { get; set; }
    
    public string? SortBy { get; set; } 
    public string? SortDirection { get; set; }
}