using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Controllers;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    public class LikesController(ILikeRepository likesRepository): BaseApiController
    {
       [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetmemberId)
        {
            var sourceMemberId = User.GetMemberId();
            if(sourceMemberId == targetmemberId) return BadRequest("You can not like yourself");
            var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetmemberId);
            if(existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberid = targetmemberId,
                };
                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }
            if(await likesRepository.SaveAllChanges()) return Ok();
            return BadRequest("Failed to update like");
        }
        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            return Ok(await likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetmemberLikes(string predicate)
        {
            var members = await likesRepository.GetMemberLikes(predicate, User.GetMemberId());
            return Ok(members);
        } 
    }

}