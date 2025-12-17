using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassHub.Data;
using ClassHub.DTOs;
using ClassHub.Models;
using System.Security.Claims;

namespace ClassHub.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/org/{orgId}/groups")]
    public class GroupController : ControllerBase
    {
        private readonly ExternalDbContext _context;

        public GroupController(ExternalDbContext context)
        {
            _context = context;
        }

         private async Task<bool> IsUserInOrganisation(int userId, int orgId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.OrganisationId == orgId);
        }

        // POST: api/org/{orgId}/groups
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroup(int orgId, [FromBody] CreateGroupDto dto)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);

            var isMember = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.OrganisationId == orgId);

            if (!isMember)
                return StatusCode(403, "Nem vagy a szervezet tagja.");

            var group = new Group
            {
                Name = dto.Name,
                OrganisationId = orgId
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // creator automatikusan group tag
            _context.GroupUsers.Add(new GroupUser
            {
                GroupId = group.Id,
                UserId = userId
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                group.Id,
                group.Name
            });
        }

        // GET: api/org/{orgId}/groups
        [HttpGet]
        public async Task<IActionResult> GetGroups(int orgId)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);

            var isMember = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.OrganisationId == orgId);

            if (!isMember)
                return Forbid();

            var groups = await _context.Groups
                .Where(g => g.OrganisationId == orgId)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            return Ok(groups);
        }

        // POST: api/org/{orgId}/groups/{groupId}
        [HttpDelete("{groupId}")]
        [Authorize]
        public async Task<IActionResult> DeleteGroup(int orgId, int groupId)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);

            var group = await _context.Groups
                .Include(g => g.GroupUsers)
                .FirstOrDefaultAsync(g => g.Id == groupId && g.OrganisationId == orgId);

            if (group == null)
                return NotFound("A csoport nem létezik.");

            var userRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.OrganisationId == orgId);

            if (userRole == null)
                return Forbid("Nem vagy a szervezet tagja.");

            if (userRole.Role.Name != "Owner")
                return Forbid("Nincs jogosultságod a csoport törléséhez");

                
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();


            return Ok(new { message = "A csoport sikeresen törölve.", groupId = group.Id });
        }
    }
}
