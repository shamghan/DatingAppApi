using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DatingAppApi.SignalR
{
    [Authorize]
    public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");

            var groupName = GetGroupName(GetUserId(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var messages = await messageRepository.GetMessageThread(GetUserId(),otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
             var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
            var recipient  =await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if(recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
                throw new HubException("Can not send message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content,
               
            };
            messageRepository.AddMessage(message);

            if(await messageRepository.SaveAllChanges())
            {
                
                var group = GetGroupName(sender.Id, recipient.Id);
                await Clients.Group(group).SendAsync("NewMessage", message.ToDto());
            }

        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Can not get member id");
            
        }
        private static string GetGroupName(string? caller, string? otherUser)
        {
            var stringCompare = string.CompareOrdinal(caller, otherUser) <0 ;
            return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
        }
    }
}