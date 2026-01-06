namespace ClassHub.DTOs
{
    public class CreateMessageDto
    {
        public string? Text { get; set; }

        public List<IFormFile>? Attachments { get; set; }
    }
}