using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingAppApi.Entities;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{

    public class MemberRepository(AppDbContext context) : IMemberRepository
    {

        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;
        }
        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await context.Members.ToListAsync();
        }
        public async Task<Member?> GetMemberByIdAsync(string id)
        {
            return await context.Members.FindAsync(id);
        }
        public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
        {
            var query = context.Members
                .Where(x => x.Id == memberId)
                .SelectMany(x => x.Photos);

            // if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.ToListAsync();
        }


        // public async Task<Member?> GetMemberForUpdate(string id)
        // {
        //     return await context.Members
        //         .Include(x => x.User)
        //         .Include(x => x.Photos)
        //         .IgnoreQueryFilters()
        //         .SingleOrDefaultAsync(x => x.Id == id);
        // }

        // public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
        // {
        //     var query = context.Members.AsQueryable();

        //     query = query.Where(x => x.Id != memberParams.CurrentMemberId);

        //     if (memberParams.Gender != null)
        //     {
        //         query = query.Where(x => x.Gender == memberParams.Gender);
        //     }

        //     var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
        //     var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));

        //     query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        //     query = memberParams.OrderBy switch
        //     {
        //         "created" => query.OrderByDescending(x => x.Created),
        //         _ => query.OrderByDescending(x => x.LastActive)
        //     };

        //     return await PaginationHelper.CreateAsync(query,
        //             memberParams.PageNumber, memberParams.PageSize);
        // }
    }

}