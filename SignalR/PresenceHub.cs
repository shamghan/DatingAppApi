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
    //user can know which users are online
    [Authorize]
    public class PresenceHub(PresenceTracker presenceTracker) :  Hub
    {
        
        public override async Task OnConnectedAsync()
        {
            await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);

            //Clients.Others- this is everybody else apart from the client that's connecting to this signal r hub
            // we will send notifcation to others 
            //UserOnline method client is going to listen for to recieve the notification that user is online
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