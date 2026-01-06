using Microsoft.Net.Http.Headers;

namespace ClassHub.DTOs
{
    public class MessageAttachmentDto
    {
        public int Id  { get; set; }
        public string Url { get; set; } = null!;
        public string Mime { get; set; } = null!;
        public long? SizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}