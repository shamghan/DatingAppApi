using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;

namespace DatingAppApi.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage (Message message);
        void DeleteMessage (Message message);
        Task<Message?> GetMessage(string messageId);
        Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParam param);
        Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientMemberId);
        Task<bool> SaveAllChanges();
    }
}