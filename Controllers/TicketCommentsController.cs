using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KillBug.Classes;
using KillBug.Models;
using Microsoft.AspNet.Identity;

namespace KillBug.Controllers
{
    [Authorize]
    [RoutePrefix("Comments")]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        NotificationHelper notifications = new NotificationHelper();

        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Comment,TicketId")] TicketComment ticketComment)
        {
            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(ticketComment.TicketId);
            if ((userId == ticket.SubmitterId) || (userId == ticket.DeveloperId) || (userId == ticket.Project.ProjectManagerId) || (ticket.Project.Users.Any(u => u.Id == userId)) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    ticketComment.UserId = User.Identity.GetUserId();
                    ticketComment.Created = DateTime.Now;

                    db.TicketComments.Add(ticketComment);
                    db.SaveChanges();

                    var user = db.Users.Find(User.Identity.GetUserId());
                    Notification newNotification = new Notification
                    {
                        Created = DateTime.Now,
                        TicketId = ticket.Id,
                        SenderId = User.Identity.GetUserId(),
                        Subject = "New Comment",
                        Body = $"Theres a new Comment on one of your tickets! <br/>Ticket: { ticket.Title }<br/>Comment: {ticket.Comments.OrderByDescending(c => c.Created).FirstOrDefault().Comment }<br/>By: { user.FullNamePosition }"
                    };
                    notifications.TicketUpdateNotification(newNotification, ticket);
                }

                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            } else
            {
                return RedirectToAction("Error", "Tickets", new { message = TicketsController.TicketError.NotAuthorizedToComment });
            }
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
