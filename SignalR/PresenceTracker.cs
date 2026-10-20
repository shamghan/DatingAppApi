using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DatingAppApi.SignalR
{
    public class PresenceTracker 
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> OnlineUsers = new();
        public Task UserConnected(string userId, string connectionId)
        {
            var connections = OnlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
            connections.TryAdd(connectionId, 0);
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userId, string connectionId)
        {
            if(OnlineUsers.TryGetValue(userId, out var connections))
            {
                connections.TryRemove(connectionId, out _);// we can just out underscore which means we do not need to use          
                                                           // what we are passing out of this method here,     
                if (connections.IsEmpty){
                    OnlineUsers.TryRemove(userId, out _);

                }
            }
            return Task.CompletedTask;
        }
        public Task<string[]> GetOnlineUsers()
        {
            return Task.FromResult(OnlineUsers.Keys.OrderBy(k=>k).ToArray());
        }
        
    }
}