using Aircraftapi.Data;
using Aircraftapi.Models;
using Aircraftapi.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Aircraftapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        [HttpPost("register")]
            public async Task<IActionResult> Register([FromForm] RegisterDto dto)
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                    return BadRequest("Username already exists.");

                CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                string imagePath = null;
                if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
                {
               
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfileImages");
                    Directory.CreateDirectory(folder); 

                    var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);
                    var filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ProfileImage.CopyToAsync(stream);
                    }

                    imagePath = Path.Combine("ProfileImages", fileName); // Relative path
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = dto.Role,
                    ProfileImagePath = imagePath
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Registration successful" });
            }

            [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token,
                username = user.Username,
                profileImage = string.IsNullOrEmpty(user.ProfileImagePath)
        ? null
        : $"{Request.Scheme}://{Request.Host}/{user.ProfileImagePath.Replace("\\", "/")}"
            });

        }

  
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role?? "Customer")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}
