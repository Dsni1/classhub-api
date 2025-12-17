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

        [HttpDelete("org/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrganisation(int id)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);
            
            var organisation = await _context.Organisations
                .Include(o => o.Groups)
                    .ThenInclude(g => g.GroupUsers)
                .Include(o => o.UserRoles)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organisation == null)
            {
                return NotFound("A szervezet nem létezik.");
            }

            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.OrganisationId == id);

            if (userRole == null)
                return Forbid("Nem vagy tagja a szervezetnek.");

            if (userRole.Role.Name != "Owner")
                return Forbid("Nincs jogosultságod törölni a szervezetet.");

            _context.Organisations.Remove(organisation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "A szervezet sikeresen törölve.", organisationId = organisation.Id });
        }
    }
}