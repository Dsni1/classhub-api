using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClassHub.Models;

namespace ClassHub.Models;

[Table("Messages")]
public class Message
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("chatroom_id")]
    public int ChatRoomId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("text")]
    [MaxLength(255)]
    public string Text { get; set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();

}


