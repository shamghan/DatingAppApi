using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingAppApi.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetMemberId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new Exception("User does not have a name identifier claim");
        }
    }
}