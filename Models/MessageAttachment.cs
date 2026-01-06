using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassHub.Models;

[Table("MessageAttachments")]
public class MessageAttachment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("message_id")]
    public int MessageId { get; set; }

    [ForeignKey(nameof(MessageId))]
    public Message Message { get; set; } = null!;

    [Required]
    [Column("url")]
    [MaxLength(500)]
    public string Url { get; set; } = null!;

    [Required]
    [Column("mime")]
    [MaxLength(50)]
    public string Mime { get; set; } = null!;

    [Column("size_bytes")]
    public long? SizeBytes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
