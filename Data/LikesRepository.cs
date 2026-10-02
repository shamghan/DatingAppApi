

using DatingApp.Data;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;
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
        

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParam param)
        {
            var memberId = param.MemberId;
            var predicate = param.Predicate;

            IQueryable<Member> membersQuery;
            if (predicate == "mutual")
            {
                var likeIds = await GetCurrentMemberLikeIds(memberId);
                membersQuery = context.Likes
                    .Where(l => l.TargetMemberid == memberId &&
                                likeIds.Contains(l.SourceMemberId))
                    .Select(l => l.SourceMember);
            }
            else
            {
                var query = context.Likes.AsQueryable();
                membersQuery = predicate switch
                {
                    "liked" => query
                        .Where(l => l.SourceMemberId == memberId)
                        .Select(l => l.TargetMember),
                    "likedBy" => query
                        .Where(l => l.TargetMemberid == memberId)
                        .Select(l => l.SourceMember),
                    _ => query
                        .Where(l => l.SourceMemberId == memberId)
                        .Select(l => l.TargetMember)
                };
            }

            return await PaginationHelper.CreateAsync(membersQuery, param.PageNumber, param.PageSize);
        }
        

        public async  Task<bool> SaveAllChanges()
        {
         return await context.SaveChangesAsync() > 0;
        }
    }
}