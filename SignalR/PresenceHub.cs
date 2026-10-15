using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingAppApi.SignalR
{
    [Authorize]
    public class PresenceHub :  Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserOnline", Context.User?.FindFirstValue(ClaimTypes.Email));
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Others.SendAsync("UserOffline", Context.User?.FindFirstValue(ClaimTypes.Email));
            await base.OnDisconnectedAsync(exception);
        }
    }
}