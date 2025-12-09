using ClassHub.Data;
using ClassHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ExternalDbContext _context;

        public TestController(ExternalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.Tests.ToListAsync(); // DbSet<Test> neve
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.Tests.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Test test)
        {
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = test.Id }, test);
        }
    }
}
