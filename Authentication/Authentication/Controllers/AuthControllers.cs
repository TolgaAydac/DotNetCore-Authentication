using Authentication.Models;
using Authentication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;


        public AuthController(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var userExists = await _context.Users
    .AnyAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (userExists)
                return BadRequest("Bu kullanıcı adı veya e-posta zaten kullanılıyor!");


            //Şifre Hashleme Kısmı

            string PasswordHash = HashPassword(request.Password);


            //Kullanıcıların Oluşturulduğu kısım
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = PasswordHash,
                Birthday = request.Birthday,
                Gender = request.Gender
            };

            //Veritabanına Kayıt
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt İşlemi Başarılı !");
        }

        //Login Entpoint

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı!");

            // Şifre kontrol
            var hashedInput = HashPassword(request.Password);

            if (user.Password != hashedInput)
                return Unauthorized("Şifre yanlış!");

            // Token oluşturma

            var token = GenerateJwtToken(user);

            return Ok(new { message = "Giriş başarılı", token });

        }



        //Şifre Hashleme Methodu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateJwtToken(User user)
        {
            var secret = _config["JwtSettings:Secret"]
                ?? throw new Exception("JWT Secret bulunamadı!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("id", user.Id.ToString()),
        new Claim("username", user.Username),
        new Claim("email", user.Email)
    };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }




    }




}