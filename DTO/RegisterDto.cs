using System.ComponentModel.DataAnnotations;

namespace DatingApp.DTO
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        [MinLength(4)]
        public string Password { get; set; } = "";

        [Required]
        public string? Gender {get; set;} = string.Empty;
        public DateOnly DateOfBirth {get; set;} 
        public string? City {get; set;} = string.Empty;
        public string? Country {get; set;} = string.Empty;
    }
}
