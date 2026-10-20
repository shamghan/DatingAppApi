using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingAppApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR;

namespace DatingAppApi.SignalR
{
    [Authorize]
    public class PresenceHub(PresenceTracker presenceTracker) :  Hub
    {

        public override async Task OnConnectedAsync()
        {
            await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserOnline", GetUserId());

            var currrentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currrentUsers);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserOffline", GetUserId());

             var currrentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currrentUsers);
            
            await base.OnDisconnectedAsync(exception);
        }
        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Can not get member id");
            
        }
    }
}