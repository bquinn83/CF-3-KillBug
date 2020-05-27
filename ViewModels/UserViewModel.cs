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
        private ApplicationDbContext db = new ApplicationDbContext();
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; }

        public CurrentUserInfoModel(string userId)
        {
            var user = db.Users.Find(userId);
            DisplayName = user.FullName;
            AvatarPath = user.AvatarPath;
            Role = user.UserRole();
        }
    }

    public class UserProfileViewModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutMe { get; set; }

    }
}