using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParam param)
        {
            var query = context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            query = param.Container switch
            {
                "Outbox" => query.Where(m => m.SenderId == param.MemberId && m.SenderDeleted == false),
                _ => query.Where(m => m.RecipientId == param.MemberId && m.RecipientDeleted == false)
            };
            var messageQuery = query.Select(MessageExtension.ToDtoProjectio());
            return await PaginationHelper.CreateAsync(messageQuery, param.PageNumber, param.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
        {
            await context.Messages
                .Where(m => m.RecipientId == currentMemberId && m.SenderId == recipientId
                || m.DateRead == null)
                .ExecuteUpdateAsync(m => m.SetProperty(m => m.DateRead, DateTime.UtcNow));
            return  await context.Messages
                .Where(m => m.RecipientId == currentMemberId && m.RecipientDeleted == false && m.SenderId == recipientId
                ||(m.SenderId == currentMemberId && m.SenderDeleted == false && m.RecipientId == recipientId))
                .OrderBy(m => m.MessageSent)
                .Select(MessageExtension.ToDtoProjectio())
                .ToListAsync();
        }

        public async Task<bool> SaveAllChanges()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}