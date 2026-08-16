using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.DTO
{
    public class UserDto
    {
        public required string Id {get;set;}
        public required string DisplayName {get;set;}
        public required string Email {get;set;}
        public string? ImageUrl {get;set;}
        public required string Token {get;set;}
    }
}