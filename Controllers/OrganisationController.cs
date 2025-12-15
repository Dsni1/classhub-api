using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ClassHub.Data;
using ClassHub.Models;
using ClassHub.DTOs;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class OrganisationController : ControllerBase
    {
        private readonly ExternalDbContext _context;

        public OrganisationController(ExternalDbContext context)
        {
            _context = context;
        }

        [HttpPost("org")]
        public async Task<IActionResult> CreateOrganisation(CreateOrganisationDto dto)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);

            var org = new Organisation
            {
                Name = dto.Name
            };

            _context.Organisations.Add(org);
            await _context.SaveChangesAsync();

            // 🔥 creator = owner
            var ownerRole = await _context.Roles
                .FirstAsync(r => r.Name == "Owner");

            _context.UserRoles.Add(new UserRole
            {
                UserId = userId,
                OrganisationId = org.Id,
                RoleId = ownerRole.Id
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                org.Id,
                org.Name
            });
        }

        [HttpGet("org/{id}")]
        public async Task<IActionResult> GetOrganisation(int id)
        {
            var organisation = await _context.Organisations.FindAsync(id);
            if (organisation == null)
            {
                return NotFound();
            }

            return Ok(organisation);
        }
    }
}