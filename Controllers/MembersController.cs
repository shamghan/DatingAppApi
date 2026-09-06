using DatingApp.Data;
using DatingApp.Entities;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")] //localhost/api/member
    [ApiController]
    public class MembersController(IMemberRepository memberRepository, IPhotoService _photoService) : BaseApiController
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
        [HttpGet("{id}/photos")] //localhost/api/member/id/photos
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return  Ok(await memberRepository.GetPhotosForMemberAsync(id));
        }
        [HttpPut] //localhost/api/member
        public async Task<ActionResult> UpdateMember(MemberUpdateDto updateDto)
        {
            var memberId = User.GetMemberId();
            if (memberId is null) return NotFound("No id found in token");
            
            var member = await memberRepository.GetMemberForUpdate(memberId);
            if(member is null) return NotFound("Member not found");          

            member.DisplayName = updateDto.DisplayName ?? member.DisplayName;
            member.Description = updateDto.Description ?? member.Description;
            member.City = updateDto.City ?? member.City;
            member.Country = updateDto.Country ?? member.Country;
            member.User.DisplayName = updateDto.DisplayName ?? member.User.DisplayName;
            memberRepository.Update(member);
            if(await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update member");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());
            if (member is null) return NotFound("Member not found");

            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error is not null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };
            if(member.Photos == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }
            member.Photos.Add(photo);
            if (await memberRepository.SaveAllAsync()) return photo;
            
            return BadRequest("Problem adding photo");
        }
    }
}
