using ClassHub.Data;
using ClassHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ClassHub.Services
{
    public class OrganisationInviteService
    {
        private readonly ExternalDbContext _context;

        public OrganisationInviteService(ExternalDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateInviteAsync(
            int organisationId,
            string email,
            int roleId)
        {
            // Van már aktív invite?
            var existingInvite = await _context.OrganisationInvites
                .AnyAsync(i =>
                    i.OrganisationId == organisationId &&
                    i.Email == email &&
                    !i.IsUsed &&
                    i.ExpiresAt > DateTime.UtcNow
                );

            if (existingInvite)
                throw new InvalidOperationException("Már van aktív meghívó erre az emailre.");

            var token = GenerateSecureToken();

            var invite = new OrganisationInvite
            {
                OrganisationId = organisationId,
                Email = email,
                RoleId = roleId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsUsed = false
            };

            _context.OrganisationInvites.Add(invite);
            await _context.SaveChangesAsync();

            return token;
        }

        private static string GenerateSecureToken()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(48)
            );
        }
    }
}
