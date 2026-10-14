using System.Diagnostics.CodeAnalysis;
using DatingAppApi.Entities;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Entities
{
    public class AppUser : IdentityUser
    {
        public required string DisplayName { get; set; }
        public string? ImageUrl { get; set; }
        public Member Member { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
