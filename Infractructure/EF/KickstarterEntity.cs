using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infractructure.EF;

public class KickstarterEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long ID { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Country { get; set; }
    public DateTime Launched {get; set;}
    public DateTime Deadline { get; set; }
    public int Goal {get; set;}
    public int Pledged {get; set;}
    public int Backers {get; set;}
    public string State {get; set;}
}