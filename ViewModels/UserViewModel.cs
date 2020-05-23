using KillBug.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace KillBug.ViewModels
{
    public class CurrentUserInfoModel
    {
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; }

        public CurrentUserInfoModel(string userId)
        {
            var user = db.Users.Find(userId);
            DisplayName = $"{user.FirstName} {user.LastName}";
            AvatarPath = user.AvatarPath;
            Role = userManager.GetRoles(userId).FirstOrDefault();
        }
    }

    public class UserProfileViewModel
    {

    }
}