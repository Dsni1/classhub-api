namespace ClassHub.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set;}

        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MessageAttachmentDto> Attachments { get; set; } = new();
    }
}