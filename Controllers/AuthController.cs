using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolDonation.API.Data;
using SchoolDonation.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SchoolDonation.API.DTOs;
using BCrypt.Net;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SchoolDonationContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SchoolDonationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        // Change return type from ActionResult<User> to ActionResult<UserResponseDto>
        [HttpPost("Register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserDto request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("Email already exists.");
            }

            // 1. Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 2. Create Entity
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PhoneNumber = request.PhoneNumber,
                UserRole = request.UserRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Handle Manager Logic
            if (user.UserRole == "Manager")
            {
                var manager = new Manager
                {
                    ManagerId = user.UserId,
                    EmployeeNumber = "EMP-" + Guid.NewGuid().ToString().Substring(0, 8)
                };
                _context.Managers.Add(manager);
                await _context.SaveChangesAsync();
            }

            // 4. MAP TO DTO (The Fix)
            // Instead of returning 'user', we create a clean response
            var response = new UserResponseDto
            {
                UserId = user.UserId,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Role = user.UserRole
            };

            return Ok(response);
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            // 1. Find User
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // 2. Verify Password (Hash vs Input)
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            // 3. Generate JWT Token
            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole) // Store Role inside token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs (Data Transfer Objects) - Helpers for receiving data

}