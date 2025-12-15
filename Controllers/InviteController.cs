using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClassHub.Data;
using ClassHub.DTOs;
using ClassHub.Services;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class OrganisationInviteController : ControllerBase
    {
        private readonly ExternalDbContext _context;
        private readonly OrganisationInviteService _inviteService;

        public OrganisationInviteController(
            ExternalDbContext context,
            OrganisationInviteService inviteService)
        {
            _context = context;
            _inviteService = inviteService;
        }

        // meghívó generálása
        [HttpPost("org/{orgId}/invite")]
        public async Task<IActionResult> InviteUser(
            int orgId,
            [FromBody] InviteUserDto dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue("userId")!);

            // 1️⃣ Jogosultság: Owner vagy Admin?
            var canInvite = await _context.UserRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur =>
                    ur.UserId == currentUserId &&
                    ur.OrganisationId == orgId &&
                    (ur.Role.Name == "Owner" || ur.Role.Name == "Admin")
                );

            if (!canInvite)
                return Forbid("Nincs jogosultság meghívni felhasználókat.");

            // 2️⃣ Role ellenőrzés
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null)
                return BadRequest("Érvénytelen role.");

            try
            {
                // 3️⃣ Invite létrehozása
                var token = await _inviteService.CreateInviteAsync(
                    orgId,
                    dto.Email,
                    role.Id
                );

                // 🔜 később: email küldés ide
                return Ok(new
                {
                    Message = "Meghívó létrehozva",
                    InviteToken = token
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // meghívó elfogadása
        [HttpPost("invite/accept")]
        public async Task<IActionResult> AcceptInvite(
            [FromBody] AcceptInviteDto dto)
        {
            var userId = int.Parse(User.FindFirstValue("userId")!);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest("A felhasználónak nincs email címe.");

            var invite = await _context.OrganisationInvites
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i =>
                    i.Token == dto.Token &&
                    !i.IsUsed &&
                    i.ExpiresAt > DateTime.UtcNow
                );

            if (invite == null)
                return BadRequest("Érvénytelen vagy lejárt meghívó.");

            // 1️⃣ Email egyezik?
            if (!string.Equals(invite.Email, userEmail, StringComparison.OrdinalIgnoreCase))
                return Forbid("Ez a meghívó nem a te email címedre szól.");

            // 2️⃣ Már tag?
            var alreadyMember = await _context.UserRoles
                .AnyAsync(ur =>
                    ur.UserId == userId &&
                    ur.OrganisationId == invite.OrganisationId
                );

            if (alreadyMember)
                return BadRequest("Már tagja vagy a szervezetnek.");

            // 3️⃣ Felvétel
            _context.UserRoles.Add(new Models.UserRole
            {
                UserId = userId,
                OrganisationId = invite.OrganisationId,
                RoleId = invite.RoleId
            });

            invite.IsUsed = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Sikeresen csatlakoztál a szervezethez."
            });
        }
    }
}
