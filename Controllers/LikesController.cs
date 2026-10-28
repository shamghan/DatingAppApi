using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Controllers;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    public class LikesController(IUnitOfWork unitOfWork): BaseApiController
    {
       [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetmemberId)
        {
            var sourceMemberId = User.GetMemberId();
            if(sourceMemberId == targetmemberId) return BadRequest("You can not like yourself");
            var existingLike = await unitOfWork.likeRepository.GetMemberLike(sourceMemberId, targetmemberId);
            if(existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberid = targetmemberId,
                };
                unitOfWork.likeRepository.AddLike(like);
            }
            else
            {
                unitOfWork.likeRepository.DeleteLike(existingLike);
            }
            if(await unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to update like");
        }
        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            return Ok(await unitOfWork.likeRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetmemberLikes([FromQuery] LikesParam param)
        {
            param.MemberId = User.GetMemberId();
            var members = await unitOfWork.likeRepository.GetMemberLikes(param);
            return Ok(members);
        } 
    }

}