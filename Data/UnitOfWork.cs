using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IMemberRepository?  _memberRepository;
        private IMessageRepository?  _messageRepository;
        private ILikeRepository?  _likeRepository;
        public IMemberRepository memberRepository => _memberRepository 
            ??= new MemberRepository(context);

        public IMessageRepository messageRepository => _messageRepository ??= new MessageRepository(context);

        public ILikeRepository likeRepository => _likeRepository  ??= new LikesRepository(context);

        public async Task<bool> Complete()
        {
            try
            {
                return await context.SaveChangesAsync() >0;
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("An error occured while saving changes", ex);
            }
        }

        public bool HasChanges()
        {
           return context.ChangeTracker.HasChanges();
        }
    }
}