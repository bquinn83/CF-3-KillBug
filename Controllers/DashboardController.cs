using KillBug.Classes;
using KillBug.Models;
using KillBug.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KillBug.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

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
            var viewData = new ProjectsDashViewModel();

            if (User.IsInRole("Admin"))
            {
                viewData.ProjectManagerId = new SelectList(rolesHelper.UsersInRole("Project Manager"), "Id", "FullName");
            }
            return View(viewData);
        }

        // GET: Tickets
        public ActionResult Tickets()
        {
            var viewData = new TicketsDashViewModel();
            var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());

            if (myProjects.Count > 0)
            {
                viewData.ProjectId = new SelectList(myProjects, "Id", "Name");
                viewData.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                viewData.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");

                ViewBag.Create = true;
            } 
            else
            {
                ViewBag.Create = false;
            }

            var charts = new ChartsController();

            var serializer = new JavaScriptSerializer();
            viewData.JsonData = serializer.Serialize(charts.TicketPriorityChartData());


            viewData.PriorityData = charts.TicketPriorityChartData();
             
            return View(viewData);
        }
    }
}