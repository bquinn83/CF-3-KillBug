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
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutMe { get; set; }

        public CurrentUserInfoModel(string userId)
        {
            var user = db.Users.Find(userId);
            DisplayName = $"{user.FirstName} {user.LastName}";
            AvatarPath = user.AvatarPath;
            Role = userManager.GetRoles(userId).FirstOrDefault();
            Address = "Demo Street 123, Demo City 04312, NJ";
            PhoneNumber = "610-555-3708";
            AboutMe = "Web Designer / UX / Graphic Artist / Coffee Lover";
        }
    }

    public class UserProfileViewModel
    {

    }
}