using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Entities;

namespace DatingAppApi.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        string GenerateRefreshToken();
    }
}