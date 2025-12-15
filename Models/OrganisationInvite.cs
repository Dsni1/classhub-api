using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassHub.Models;

namespace ClassHub.Models;

[Table("OrganisationInvites")]
public class OrganisationInvite
{
    public int Id { get; set; }

    [Required]
    public int OrganisationId { get; set; }
    public Organisation Organisation { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public int RoleId { get; set; }
    public Role Role { get; set; }

    [Required]
    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }
}
