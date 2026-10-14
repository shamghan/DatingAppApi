using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Entities;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> context,ITokenService tokenService) : BaseApiController
    {
        [HttpPost("{email}")]
        public async Task<ActionResult<UserDto>> Delete(string email)
        {
            
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return NotFound("User not found");
            await context.DeleteAsync(user);
            return Ok("User deleted successfully");
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Member = new Member
                {
                    DisplayName = registerDto.DisplayName,
                    Gender = registerDto.Gender,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    DateOfBirth = registerDto.DateOfBirth,
                }
            };
            var result =await context.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded) 
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Identity", error.Description);
                }
                    return ValidationProblem();
            }
            var roleResult = await context.AddToRoleAsync(user, "Member");
            await setRefreshToknCookie(user);
            return await user.ToDto(tokenService);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user= await context.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized("Invalid Email");

            var result = await context.CheckPasswordAsync(user, loginDto.Password);
            if(!result) return Unauthorized("Invalid Password");
            await setRefreshToknCookie(user);

            return await user.ToDto(tokenService);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(refreshToken == null) return NoContent();

            var user = await context.Users.FirstOrDefaultAsync(x=>x.RefreshToken == refreshToken && x.RefreshTokenExpiry>DateTime.UtcNow);
            if(user == null) return Unauthorized();
            await setRefreshToknCookie(user);
            return await user.ToDto(tokenService);
        }
        private async Task setRefreshToknCookie(AppUser user)
        {
            var refreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await context.UpdateAsync(user);
            var cookieOptions= new CookieOptions
            {
                HttpOnly=true ,
                Secure =true,
                SameSite=Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                Expires=DateTime.UtcNow.AddDays(7),

            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
