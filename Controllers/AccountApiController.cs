using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController(AppDbContext context) : BaseApiController
    {

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await EmailExists(registerDto.Email)) return BadRequest("Email Taken");
            var hmac = new HMACSHA512();
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
        private async Task<bool> EmailExists(string email)
        {
            return await context.Users.AnyAsync(x=>x.Email.ToLower() == email.ToLower());
        }
    }
}
