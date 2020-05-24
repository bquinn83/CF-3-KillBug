using KillBug.Models;
using KillBug.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KillBug.Classes;

namespace KillBug.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Dashboard()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            UserRolesHelper rolesHelper = new UserRolesHelper();

            DashboardViewModel viewData = new DashboardViewModel()
            {
                TeamCount = db.Users.Count(),
                PMCount = rolesHelper.UsersInRole("Project Manager").Count,
                DevCount = rolesHelper.UsersInRole("Developer").Count,
                SubCount = rolesHelper.UsersInRole("Submitter").Count,
                ProjectsCount = db.Projects.Count(),
                TicketsCount = db.Tickets.Count(),
                NewTicketsCount = db.Tickets.Where(t => t.TicketStatus.Name == "New").Count(),
                CommentsCount = db.TicketComments.Count(),
                AttachmentsCount = db.TicketAttachments.Count(),
                HistoryCount = db.TicketHistories.Count()
            };

            return View(viewData);
        }
    }
}