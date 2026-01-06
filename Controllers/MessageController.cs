using System.Security.Claims;
using ClassHub.Data;
using ClassHub.DTOs;
using ClassHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.Controllers;

[ApiController]
[Route("api/chatrooms/{chatRoomId:int}/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ExternalDbContext _db;
    private readonly IWebHostEnvironment _env;

    // Állítható limitek
    private static readonly HashSet<string> AllowedImageMimes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 200;

    public MessagesController(ExternalDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // GET: api/chatrooms/{chatRoomId}/messages?skip=0&take=50
    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        int chatRoomId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = DefaultPageSize)
    {
        take = Math.Clamp(take, 1, MaxPageSize);
        skip = Math.Max(0, skip);

        var userId = GetUserId();

        // jogosultság: tagja-e a chatnek?
        var isMember = await _db.ChatRoomUsers
            .AnyAsync(x => x.ChatRoomId == chatRoomId && x.UserId == userId);

        if (!isMember)
            return Forbid();

        var messages = await _db.Messages
            .AsNoTracking()
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Include(m => m.Attachments)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ChatRoomId = m.ChatRoomId,
                UserId = m.UserId,
                Text = m.Text,
                CreatedAt = m.CreatedAt,
                Attachments = m.Attachments
                    .OrderBy(a => a.Id)
                    .Select(a => new MessageAttachmentDto
                    {
                        Id = a.Id,
                        Url = a.Url,
                        Mime = a.Mime,
                        SizeBytes = a.SizeBytes,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(messages);
    }
    
    // POST: api/chatrooms/{chatRoomId}/messages
    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)] // pl. 25MB request limit
    public async Task<ActionResult<MessageDto>> CreateMessage(
        int chatRoomId,
        [FromForm] CreateMessageDto request)
    {
        var userId = GetUserId();

        // jogosultság: tagja-e a chatnek?
        var isMember = await _db.ChatRoomUsers
            .AnyAsync(x => x.ChatRoomId == chatRoomId && x.UserId == userId);

        if (!isMember)
            return Forbid();

        var hasText = !string.IsNullOrWhiteSpace(request.Text);
        var hasFiles = request.Attachments != null && request.Attachments.Count > 0;

        if (!hasText && !hasFiles)
            return BadRequest("Text or at least one image file is required.");

        if (hasFiles)
        {
            foreach (var file in request.Attachments!)
            {
                if (file.Length <= 0)
                    return BadRequest("One of the files is empty.");

                if (file.Length > MaxFileSizeBytes)
                    return BadRequest($"File too large. Max {MaxFileSizeBytes / (1024 * 1024)}MB allowed.");

                if (!AllowedImageMimes.Contains(file.ContentType))
                    return BadRequest($"Unsupported image type: {file.ContentType}");
            }
        }

        //Message mentése 
        var message = new Message
        {
            ChatRoomId = chatRoomId,
            UserId = userId,
            Text = hasText ? request.Text!.Trim() : null, 
            CreatedAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        // 2) Fájlok mentése + attachments rekordok
        var attachments = new List<MessageAttachment>();

        if (hasFiles)
        {
            var uploadRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "messages", message.Id.ToString());
            Directory.CreateDirectory(uploadRoot);

            foreach (var file in request.Attachments!)
            {
                var ext = GuessExtension(file.ContentType) ?? Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(ext))
                    ext = ".bin";

                var safeFileName = $"{Guid.NewGuid():N}{ext}";
                var absolutePath = Path.Combine(uploadRoot, safeFileName);

                await using (var stream = System.IO.File.Create(absolutePath))
                {
                    await file.CopyToAsync(stream);
                }

                // URL amit a kliens használ (relative)
                var relativeUrl = $"/uploads/messages/{message.Id}/{safeFileName}";

                attachments.Add(new MessageAttachment
                {
                    MessageId = message.Id,
                    Url = relativeUrl,
                    Mime = file.ContentType,
                    SizeBytes = file.Length,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _db.MessageAttachments.AddRange(attachments);
            await _db.SaveChangesAsync();
        }

        // 3) Válasz DTO
        var result = new MessageDto
        {
            Id = message.Id,
            ChatRoomId = message.ChatRoomId,
            UserId = message.UserId,
            Text = message.Text,
            CreatedAt = message.CreatedAt,
            Attachments = attachments.Select(a => new MessageAttachmentDto
            {
                Id = a.Id,
                Url = a.Url,
                Mime = a.Mime,
                SizeBytes = a.SizeBytes,
                CreatedAt = a.CreatedAt
            }).ToList()
        };

        return CreatedAtAction(nameof(GetMessages), new { chatRoomId }, result);
    }

    private int GetUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub")
                  ?? User.FindFirstValue("userId");

        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("User id claim missing.");

        return userId;
    }

    private static string? GuessExtension(string mime)
    {
        return mime.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };
    }
}
