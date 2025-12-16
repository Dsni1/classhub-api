using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassHub.Data;
using ClassHub.Models;
using ClassHub.DTOs;
using Microsoft.AspNetCore.Identity;
using ClassHub.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly ExternalDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;

        public LoginController(ExternalDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
                return Unauthorized("Invalid username or password");

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid username or password");

            // Generate JWT
            var jwtToken = _jwtService.GenerateToken(user, request.RememberMe);

            var refreshToken = _jwtService.GenerateRefreshToken(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Request.Headers["User-Agent"].ToString()
            );

            refreshToken.UserId = user.Id;

            // Save refresh token to DB
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                UserId = user.Id,
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            });
        }


        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenValidateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest("Token is required");

            try
            {
                var key = Environment.GetEnvironmentVariable("JWT_KEY");
                var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
                var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key)
                    ),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(
                    request.Token,
                    validationParameters,
                    out _
                );

                var userId = principal.Claims.First(c => c.Type == "userId").Value;

                return Ok(new
                {
                    valid = true,
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    valid = false,
                    message = "Invalid or expired token",
                    error = ex.Message
                });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required");

            var storedToken = await _context.RefreshTokens
                .Include(t => t.User)
                .SingleOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null)
                return Unauthorized("Invalid refresh token");

            if (!storedToken.IsActive)
                return Unauthorized("Refresh token is expired or revoked");

            // régi token érvénytelenítése
            storedToken.RevokedAt = DateTime.UtcNow;

            // új token
            var newRefreshToken = _jwtService.GenerateRefreshToken(
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Request.Headers["User-Agent"].ToString()
            );

            newRefreshToken.UserId = storedToken.UserId;
            newRefreshToken.ReplacesToken = storedToken.Token;

            _context.RefreshTokens.Add(newRefreshToken);

            // új jwt
            var jwt = _jwtService.GenerateToken(storedToken.User, rememberMe: false);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = jwt,
                RefreshToken = newRefreshToken.Token
            });
        }

        [HttpPost("logout")]
        private  async Task<IActionResult> Logout([FromBody] RefreshRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required");

            var storedToken = await _context.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == request.RefreshToken);

            if (storedToken == null)
                return Unauthorized("Invalid refresh token");

            if (!storedToken.IsActive)
                return Unauthorized("Refresh token is already expired or revoked");

            // Token érvénytelenítése
            storedToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out successfully" });
        }

    }
}
