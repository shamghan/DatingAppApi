using DatingApp.Entities;
using DatingAppApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatingApp.Data
{
   public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<AppUser> Users { get; set; }

        public DbSet<Member> Members { get; set;}
        public DbSet<Photo> Photos { get; set;}
        public DbSet<MemberLike> Likes { get; set;}
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Message>()
                .HasOne(e => e.Sender)
                .WithMany(e => e.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(e => e.Sender)
                .WithMany(e => e.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

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

            var nullableDateTimeConverter = new ValueConverter<DateTime?,DateTime?>(
                v=> v.HasValue ? v.Value.ToUniversalTime() : null,
                v=> v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
            );

            foreach(var enttyTyp in modelBuilder.Model.GetEntityTypes())
            {
                foreach(var property in enttyTyp.GetProperties())
                {
                    if(property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                    else if(property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);    
                    }
                }
            }
        }

    }
}
