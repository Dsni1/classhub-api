using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassHub.Models;

namespace ClassHub.Models;


[Table("Groups")]
public class Group
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("organisation_id")]
    public int OrganisationId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [ForeignKey(nameof(OrganisationId))]
    public Organisation Organisation { get; set; } = null!;

    public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
}
