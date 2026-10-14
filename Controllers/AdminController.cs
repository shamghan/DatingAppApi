using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Controllers;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users.ToListAsync();
            var usersWithRoles = new List<object>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles.ToList()
                });
            }
            return Ok(usersWithRoles);
        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosToModerate()
        {
            return Ok("Admin or Moderator can see this");
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userId,[FromQuery]string roles)
        {
            if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Could not retrieve user");
            var userRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return BadRequest("Failed to add roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove roles");

            return Ok(await userManager.GetRolesAsync(user));
        }
        
    }
}