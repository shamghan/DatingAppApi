using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.DTO
{
    public class CreateMessageDto
    {
        public required string RecipientId { get; set;} = string.Empty;
        public required string Content { get; set;} = string.Empty;
    }
}