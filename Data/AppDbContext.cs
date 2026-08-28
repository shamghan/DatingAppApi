using DatingApp.Entities;
using DatingAppApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }

        public DbSet<Member> Members { get; set;}
        public DbSet<Photo> Photos { get; set;}
    }
}
