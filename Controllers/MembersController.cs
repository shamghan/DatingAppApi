using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers
{
    [Route("api/[controller]")] //localhost/api/member
    [ApiController]
    public class MembersController(AppDbContext Context) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await Context.Users.ToListAsync();
            return members;
        }
        [HttpGet("{id}")] //localhost/api/member/id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await Context.Users.FindAsync(id);
            if (member is null) return NotFound();
            return member;
        }
        
    }

}
