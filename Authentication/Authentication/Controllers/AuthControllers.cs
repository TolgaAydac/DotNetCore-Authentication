using Authentication.Models;
using Authentication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;


        public AuthController(AuthDbContext context)
        {
            _context = context;
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
                Password = request.Password,
                Birthday = request.Birthday,
                Gender = request.Gender
            };

            //Veritabanına Kayıt
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kayıt İşlemi Başarılı !");
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
    }




}