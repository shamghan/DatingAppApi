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
    public class MessageHub(IUnitOfWork unitOfWork, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");

            var groupName = GetGroupName(GetUserId(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);
            var messages = await unitOfWork.messageRepository.GetMessageThread(GetUserId(),otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await unitOfWork.messageRepository.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async Task<bool> AddToGroup(string groupName)
        {
            var group = await unitOfWork.messageRepository.GetMessageGroup(groupName);
            var connection =  new Connection(Context.ConnectionId, GetUserId());
            if(group == null)
            {
                group = new Group(name:groupName);
                unitOfWork.messageRepository.AddGroup(group);

            }
            group.Connections.Add(connection);
            return await unitOfWork.Complete();
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
             var sender = await unitOfWork.memberRepository.GetMemberByIdAsync(GetUserId());
            var recipient  =await unitOfWork.memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if(recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
                throw new HubException("Can not send message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content,
               
            };
            
            var groupName = GetGroupName(sender.Id,recipient.Id);
            var group = await unitOfWork.messageRepository.GetMessageGroup(groupName);
            var userInGroup = group!=null && group.Connections.Any(x=>x.UserId == message.RecipientId);
            if(userInGroup)
            {
                message.DateRead=DateTime.UtcNow;
            }
            
            unitOfWork.messageRepository.AddMessage(message);

            if(await unitOfWork.Complete())
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
                if(connections != null && connections.Count>0 && !userInGroup)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto());
                }
            }

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