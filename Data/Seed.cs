using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Entities;
using DatingAppApi.DTO;
using DatingAppApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class Seed
    {
        public static  async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new Dictionary<string, string>
            {
                ["member-id"] = "Member",
                ["admin-id"] = "Admin",
                ["moderator-id"] = "Moderator",
            };
            foreach (var (roleId, roleName) in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole { Id = roleId, Name = roleName });
                }
            }

            var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);
            if(members == null || members.Count == 0)
            {
                Console.WriteLine("No members in seed data");
                return;
            }

            foreach(var member in members)
            {
                var existing = await userManager.FindByEmailAsync(member.Email);
                if (existing is not null)
                {
                    if (!await userManager.IsInRoleAsync(existing, "Member"))
                    {
                        await userManager.AddToRoleAsync(existing, "Member");
                    }
                    continue;
                }

                var user= new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    DisplayName = member.DisplayName,
                    ImageUrl = member.ImageUrl,
                    UserName = member.Email,
                    Member= new Member{
                        Id= member.Id,
                        DisplayName = member.DisplayName,
                        Description= member.Description,
                        DateOfBirth= member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created,
                    }
                };
                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id,
                   // IsApproved = true
                });

                var result = await userManager.CreateAsync(user, "Pa$$w0rd");

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Failed to seed {member.Email}: {error.Description}");
                    }
                    continue;
                }
                await userManager.AddToRoleAsync(user, "Member");
            }
            var admin = await userManager.FindByEmailAsync("admin@test.com");
            if (admin == null)
            {
                admin = new AppUser
                {
                    DisplayName = "Admin",
                    Email = "admin@test.com",
                    UserName = "admin@test.com"
                };
                var adminResult = await userManager.CreateAsync(admin, "Pa$$w0rd");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, ["Admin","Moderator"]);
                }
            }
        }
    }
}