using KillBug.Classes;
using KillBug.Models;
using KillBug.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KillBug.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Main
        public ActionResult Main()
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


        // GET: Projects
        public ActionResult Projects()
        {
            return View();
        }

        // GET: Tickets
        public ActionResult Tickets()
        {
            return View();
        }
    }
}