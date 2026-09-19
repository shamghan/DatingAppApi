

using DatingApp.Data;
using DatingAppApi.Entities;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class LikesRepository(AppDbContext context) : ILikeRepository
    {
        public void AddLike(MemberLike like)
        {
            context.Likes.Add(like);
        }

        public void DeleteLike(MemberLike like)
        {
            context.Likes.Remove(like);
        }

        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
            => await context.Likes
                            .Where(l => l.SourceMemberId == memberId)
                            .Select(l => l.TargetMemberid)
                            .ToListAsync();
            

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
            => await context.Likes.FindAsync(sourceMemberId, targetMemberId);
        

        public async Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
        {
            var query = context.Likes.AsQueryable();
            return predicate switch
            {
                "liked" => await query
                        .Where(l => l.SourceMemberId == memberId)
                        .Select(l => l.TargetMember)
                        .ToListAsync(),
                "likedBy" => await query
                        .Where(l => l.TargetMemberid == memberId)
                        .Select(l => l.SourceMember)
                        .ToListAsync(),
                // default://mutual
                _ => await GetMutualLikes(query, memberId)
            };

        }
        private async Task<IReadOnlyList<Member>> GetMutualLikes(
            IQueryable<MemberLike> query,
            string memberId)
        {
            var likeIds = await GetCurrentMemberLikeIds(memberId);

            return await query
                .Where(x => x.TargetMemberid == memberId &&
                            likeIds.Contains(x.SourceMemberId))
                .Select(l => l.SourceMember)
                .ToListAsync();
        }

        public async  Task<bool> SaveAllChanges()
        {
         return await context.SaveChangesAsync() > 0;
        }
    }
}