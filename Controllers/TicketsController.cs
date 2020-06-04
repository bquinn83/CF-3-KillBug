using KillBug.Models;
using KillBug.Classes;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KillBug.ViewModels;

namespace KillBug.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private TicketsHelper ticketHelper = new TicketsHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private NotificationHelper notificationHelper = new NotificationHelper();

        // GET: Tickets
        public ActionResult AllTickets()
        {
            var ticketIndexVMs = new List<TicketsIndexViewModel>();
            var allTickets = db.Tickets.ToList();
            foreach (var ticket in allTickets)
            {
                ticketIndexVMs.Add(new TicketsIndexViewModel
                {
                    Ticket = ticket,
                    TicketStatus = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId),

                });
            }

            return View(ticketIndexVMs);
        }

        // GET :Tickets/MyTickets
        [Authorize]
        public ActionResult MyTickets()
        {
            return View(ticketHelper.ListMyTickets());
        }

        // GET: Tickets/Details
        [Authorize]
        public ActionResult Details(int? id)
        {
            var userId = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return RedirectToAction("Error", new { message = TicketError.NullTicket });
            }

            if ((userId == ticket.SubmitterId) || (userId == ticket.DeveloperId) || (userId == ticket.Project.ProjectManagerId) || (ticket.Project.Users.Any(u => u.Id == userId)) || User.IsInRole("Admin"))
            {
                ViewBag.DeveloperId = new SelectList(ticketHelper.AssignableDevelopers(ticket.ProjectId), "Id", "FullNamePosition", ticket.DeveloperId);

                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Error", new { message = TicketError.NotAuthorizedToView });
            }
        }

        // GET: Tickets/History
        [Authorize]
        public ActionResult History()
        {
            //var userId = User.Identity.GetUserId();
            //var ticketHistories = new List<TicketHistory>();

            //if (User.IsInRole("Submitter"))
            //{
            //    ticketHistories = db.TicketHistories.Include(t => t.Ticket).Include(t => t.User).Where(t => t.Ticket.SubmitterId == userId).ToList();
            //}
            //return View(ticketHistories);
            return View(HistoryHelper.ListMyHistory());
        }

        [Authorize(Roles = "Submitter")]
        // GET: Tickets/Create
        public ActionResult Create()
        {
            var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            if (myProjects.Count > 0)
            {
                ViewBag.ProjectId = new SelectList(myProjects, "Id", "Name");
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");

                return View();
            }
            return RedirectToAction("Error", new { message = TicketError.NoProjects });
        }

        // POST: Tickets/Create
        [Authorize(Roles = "Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,TicketTypeId,TicketPriorityId,Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.SubmitterId = User.Identity.GetUserId();
                ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(t => t.Name == "New").Id;
                ticket.Created = DateTime.Now;

                db.Tickets.Add(ticket);
                db.SaveChanges();

                //New Ticket Notification:
                var user = db.Users.Find(User.Identity.GetUserId());
                var projectName = db.Projects.Find(ticket.ProjectId).Name;
                Notification newNotification = new Notification
                {
                    Created = DateTime.Now,
                    TicketId = ticket.Id,
                    SenderId = user.Id,
                    Subject = "New Ticket",
                    Body = $"One of your projects has a new ticket!<br/>Ticket: { ticket.Title }<br/>Project: { projectName }<br/>By: { user.FullNamePosition }"
                };

                notificationHelper.NewTicketNotification(newNotification, ticket);

                return RedirectToAction("Details", new { id = ticket.Id });
            }

            var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            ViewBag.ProjectId = new SelectList(myProjects, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperId,SubmitterId,Title,Description,Created,IsArchived")] Ticket ticket)
        {
            var userId = User.Identity.GetUserId();
            var project = db.Projects.Find(ticket.ProjectId);
            if ((userId == ticket.SubmitterId) || (userId == ticket.DeveloperId) || (userId == project.ProjectManagerId) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                    ticket.Updated = DateTime.Now;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                    historyHelper.CreateHistory(oldTicket, ticket);

                    notificationHelper.ManageNotifications(oldTicket, newTicket);

                    RedirectToAction("Details", new { id = ticket.Id });
                }

                ViewBag.DeveloperId = new SelectList(ticketHelper.AssignableDevelopers(ticket.ProjectId), "Id", "FullNamePosition", ticket.DeveloperId);

                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            else
            {
                return RedirectToAction("Error", new { message = TicketError.NotAuthorizedToEdit });
            }
        }

        public ActionResult Error(TicketError? message)
        {
            ViewBag.ErrorMessage = message == TicketError.NoProjects ? "You are not assigned to any Projects.<br/><br/>Please contact your superior to be assigned to a new Project."
                : message == TicketError.NotAuthorizedToEdit ? "You are not authorized to Edit this Ticket."
                : message == TicketError.NotAuthorizedToComment ? "You are not authorized to leave Comments on this Ticket."
                : message == TicketError.NotAuthorizedToUpload ? "You are not authorized to upload Attachments to this Ticket."
                : message == TicketError.NotAuthorizedToView ? "You are not authorized to view this Ticket."
                : message == TicketError.NullTicket ? "There has been an error retrieving the ticket. Please alert your superior."
                : "There has been an error. Please try again and alert management if it persists.";

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum TicketError
        {
            NoProjects,
            NotAuthorizedToEdit,
            NotAuthorizedToComment,
            NotAuthorizedToUpload,
            NotAuthorizedToView,
            NullTicket
        }
    }
}
