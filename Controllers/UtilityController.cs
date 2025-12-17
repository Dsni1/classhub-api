using Microsoft.AspNetCore.Mvc;
using ClassHub.Data;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("")]
    public class UtilityController : ControllerBase
    {
        private readonly ExternalDbContext _context;

        public UtilityController(ExternalDbContext context)
        {
            _context = context;
        }

        // GET: /health
        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok(new
                    {
                        status = "ok",
                        db = "up",
                        time = DateTime.UtcNow
                    });
            }
            catch
            {
                return StatusCode(503, "Service Unavailable");
            }
        }
    }
}