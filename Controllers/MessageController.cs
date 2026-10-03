using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Controllers;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    public class MessageController(IMessageRepository messageRepository,
    IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
            var recipient  =await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if(recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
                return BadRequest("Cannot send this messsage");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content,
               
            };
            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllChanges())
                return Ok(message.ToDto());

            return BadRequest("Failed to send message");   
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessageByIdAsync([FromQuery]MessageParam param)
        {
            param.MemberId = User.GetMemberId();
            return await messageRepository.GetMessagesForMember(param);
        }
        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
        {
            var currentMemberId = User.GetMemberId();
            return Ok(await messageRepository.GetMessageThread(currentMemberId, recipientId));
        }
    }
}