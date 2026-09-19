using DatingApp.Entities;
using DatingAppApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatingApp.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }

        public DbSet<Member> Members { get; set;}
        public DbSet<Photo> Photos { get; set;}
        public DbSet<MemberLike> Likes { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MemberLike>()
                .HasKey(e => new {e.SourceMemberId, e.TargetMemberid});

            modelBuilder.Entity<MemberLike>()
            .HasOne(e => e.SourceMember)
            .WithMany(e => e.LikedMembers)
            .HasForeignKey(e => e.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<MemberLike>()
            .HasOne(e => e.TargetMember)
            .WithMany(e => e.LikedByMembers)
            .HasForeignKey(e => e.TargetMemberid)
            .OnDelete(DeleteBehavior.NoAction);

            var dateTimeConverter = new ValueConverter<DateTime,DateTime>(
                v=> v.ToUniversalTime(),
                v=> DateTime.SpecifyKind(v, DateTimeKind.Utc)

            );

            foreach(var enttyTyp in modelBuilder.Model.GetEntityTypes())
            {
                foreach(var property in enttyTyp.GetProperties())
                {
                    if(property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
        }

    }
}
