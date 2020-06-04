using KillBug.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

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
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public int AddressZip { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutMe { get; set; }

        public UserProfileViewModel(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            AvatarPath = user.AvatarPath;
            Address = $"{ user.AddressLine1 }, { user.AddressLine2 } { user.AddressCity } { user.AddressState } { user.AddressZip }";
            AddressLine1 = user.AddressLine1;
            AddressLine2 = user.AddressLine2;
            AddressCity = user.AddressCity;
            AddressState = user.AddressState;
            AddressZip = user.AddressZip;
            PhoneNumber = user.PhoneNumber;
            AboutMe = user.AboutMe;
            Role = user.UserRole();
        }
    }
}