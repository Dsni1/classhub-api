using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.Models;

[Table("ChatRoomUsers")]
public class ChatRoomUser
{
    [Column("chatroom_id")]
    public int ChatRoomId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }
    public ChatRoom ChatRoom { get; set; }
    public User User { get; set; }
}
