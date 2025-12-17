using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassHub.Data;
using ClassHub.Models;
using ClassHub.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ExternalDbContext _context;
        private readonly PasswordHasher<User> _passwordHash;

        public UserController(ExternalDbContext context)
        {
            _context = context;
            _passwordHash = new PasswordHasher<User>();
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName
                })
                .ToList();

            return Ok(users);
        }

        // GET: api/users/{id}/organisations
        [HttpGet("{id}/organisations")]
        public IActionResult GetUserOrganisations(int id)
        {
            var data = _context.UserRoles
                .Where(ur => ur.UserId == id)
                .Include(ur => ur.Organisation)
                .Include(ur => ur.Role)
                .Select(ur => new
                {
                    OrganisationId = ur.Organisation.Id,
                    OrganisationName = ur.Organisation.Name,
                    Role = ur.Role.Name
                })
                .ToList();

            if (!data.Any())
                return NotFound("User has no organisations");

            return Ok(data);
        }

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAccount()
        {
            var userId = int.Parse(User.FindFirst("userId")!.Value);

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .Include(u => u.GroupUsers)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            _context.UserRoles.RemoveRange(user.UserRoles);
            _context.GroupUsers.RemoveRange(user.GroupUsers);
            _context.RefreshTokens.RemoveRange(user.RefreshTokens);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new { Message = "User account deleted successfully" });
        }
    }
}
