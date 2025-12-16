using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassHub.Models;

namespace ClassHub.Models;

[Table("Users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_name")]
    [MaxLength(255)]
    public string UserName { get; set; } = null!;

    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    [Column("password")]
    [MaxLength(255)]
    public string Password { get; set; } = null!;

    // kapcsolat RefreshToken-rel
    public ICollection<RefreshToken> RefreshTokens { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<GroupUser> GroupUsers { get; set; } = new List<GroupUser>();
    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; } = new List<ChatRoomUser>();

}
