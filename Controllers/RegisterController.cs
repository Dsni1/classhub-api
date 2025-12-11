using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassHub.Data;
using ClassHub.Models;
using ClassHub.DTOs;
using Microsoft.AspNetCore.Identity;
using ClassHub.Services;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api")]
    public class RegisterController : ControllerBase
    {
        private readonly ExternalDbContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher;

        public RegisterController(ExternalDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Username ellenőrzés
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
                return BadRequest("Username already taken.");

            // Email ellenőrzés
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already in use.");

            // Új user
            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email
            };

            // Jelszó hash-elés
            newUser.Password = _passwordHasher.HashPassword(newUser, request.Password);

            // Mentés DB-be
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Token generálás (auto-login)
            var token = _jwtService.GenerateToken(newUser, request.RememberMe);

            return Ok(new
            {
                Message = "User registered successfully",
                UserId = newUser.Id,
                Token = token
            });
        }
    }
}
