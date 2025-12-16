using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.Models;

[Table("ChatRooms")]
public class ChatRoom
{
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("type")]
    public string Type { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("group_id")]
    public int? GroupId { get; set; }
    public ICollection<ChatRoomUser>? ChatRoomUsers { get; set; }
}
