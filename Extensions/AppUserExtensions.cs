using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Entities;
using DatingAppApi.DTO;
using DatingAppApi.Interfaces;

namespace DatingAppApi.Extensions
{
    public static class AppUserExtensions
    {
        public static UserDto ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                //ImageUrl = user.ImageUrl,
                Token = tokenService.CreateToken(user)
            };
        }
    }
}