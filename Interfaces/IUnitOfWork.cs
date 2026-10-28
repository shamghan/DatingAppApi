using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository memberRepository {get;}
        IMessageRepository messageRepository {get;}
        ILikeRepository likeRepository {get;}
        Task<bool> Complete();
        bool HasChanges();
    }
}