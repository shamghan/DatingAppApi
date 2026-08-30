using DatingApp.Data;
using DatingApp.Entities;
using DatingAppApi.Entities;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")] //localhost/api/member
    [ApiController]
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {
       
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            var members = await memberRepository.GetMembersAsync();
            return Ok(members);
        }
        [HttpGet("{id}")] //localhost/api/member/id
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberByIdAsync(id);
            if (member is null) return NotFound();
            return member;
        }
        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return  Ok(await memberRepository.GetPhotosForMemberAsync(id));
        }
    }

}
