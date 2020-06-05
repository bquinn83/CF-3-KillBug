using KillBug.Classes;
using KillBug.Models;
using KillBug.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace KillBug.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        private readonly UserRolesHelper RolesHelper = new UserRolesHelper();
        private NotificationHelper notifications = new NotificationHelper();

        //GET: TicketSettings
        public ActionResult TicketSettings()
        {
            var model = new TicketSettingsVM
            {
                Types = db.TicketTypes.ToList(),
                Status = db.TicketStatus.ToList(),
                Priorities = db.TicketPriorities.ToList()
            };

            return View(model);
        }
        //GET: ManageRoles
        [Authorize(Roles="Admin")]
        public ActionResult ManageRoles()
        {

            var users = db.Users.ToList();
            var viewData = new List<UserRoleVM>();

            foreach (var user in users)
            {
                if (user.Id != User.Identity.GetUserId())
                {
                    viewData.Add(new UserRoleVM
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Roles = new SelectList(db.Roles, "Name", "Name", user.UserRole())
                    });
                }
            }

            ViewBag.UserIds = new MultiSelectList(db.Users.OrderBy(u => u.LastName), "Id", "FullNamePosition");
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");

            return View(viewData);
        }

        //POST: ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(List<string> userIds, string roleName)
        {
            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    var userRole = RolesHelper.ListUserRoles(userId).FirstOrDefault();
                    if (userRole != null)
                    {
                        RolesHelper.RemoveUserFromRole(userId, userRole);
                    }
                    RolesHelper.AddUserToRole(userId, roleName);

                    //send notification
                    notifications.RoleChangeNotification(userId, roleName);
                }
            }
            return RedirectToAction("ManageRoles");
        }

        // AJAX POST: UpdateUserRole
        [HttpPost]
        public JsonResult UpdateUserRole(string email, string roleName)
        {
            try
            {
                var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
                userManager.RemoveFromRole(user.Id, user.UserRole());
                userManager.AddToRole(user.Id, roleName);

                // Before returning, we must do some housekeeping:
                // If your downgrading a Project Manager, they should be removed as Project Manager for all Projects
                // If your upgrading a developer, should their tickets be re-assigned? Thats up to you!

                return Json(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(false);
            }
        }
        // GET: Admin
        public ActionResult AskAboutRoles()
        {
            if (RolesHelper.UsersInRole("Developer").Count == 0)
            {
                return View("NoDevs");
            }
            return View();
        }
        [HttpPost]
        public JsonResult AddTicketType(string type, string description)
        {
            if (type != null)
            {
                var ticket = new TicketType(type, description);
                db.TicketTypes.Add(ticket);
                db.SaveChanges();
                return Json(ticket);
            }
            else
            {
                return Json(false);
            }
        }
        [HttpPost]
        public JsonResult AddTicketStatus(string name)
        {
            if (name != null)
            {
                var status = new TicketStatus(name);
                db.TicketStatus.Add(status);
                db.SaveChanges();
                return Json(status);
            }
            else
            {
                return Json(false);
            }
        }
        [HttpPost]
        public JsonResult AddTicketPriority(string name)
        {
            if (name != null)
            {
                var priority = new TicketPriority(name);
                db.TicketPriorities.Add(priority);
                db.SaveChanges();
                return Json(priority);
            }
            else
            {
                return Json(false);
            }
        }
    }
}