using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KillBug.Classes;
using KillBug.Models;
using Microsoft.AspNet.Identity;

namespace KillBug.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NotificationHelper notificationHelper = new NotificationHelper();

        // GET: TicketAttachments
        public ActionResult Index()
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketAttachments.ToList());
        }

        // POST: TicketAttachments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload([Bind(Include = "TicketId,FilePath,Description")] TicketAttachment ticketAttachment, HttpPostedFileBase Attachment)
        {
            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(ticketAttachment.TicketId);
            if ((userId == ticket.SubmitterId) || (userId == ticket.DeveloperId) || (userId == ticket.Project.ProjectManagerId) || (ticket.Project.Users.Any(u => u.Id == userId)) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    if (Attachment != null)
                    {
                        //create the file name
                        var fileName = Path.GetFileNameWithoutExtension(Attachment.FileName);
                        fileName = StringUtilities.URLFriendly(fileName);
                        ticketAttachment.FileName = fileName + Path.GetExtension(Attachment.FileName);
                        fileName = $"{fileName}-{DateTime.Now.Ticks}";
                        fileName = $"{fileName}{Path.GetExtension(Attachment.FileName)}";

                        //create the path
                        var projId = db.Tickets.Find(ticketAttachment.TicketId).ProjectId;
                        var path = Server.MapPath($"~/Uploads/Attachments/{ projId }/{ ticketAttachment.TicketId }/");
                        Directory.CreateDirectory(Server.MapPath($"~/Uploads/Attachments/{ projId }/{ ticketAttachment.TicketId }/"));

                        //save file & ticket Attachment data
                        Attachment.SaveAs(Path.Combine(Server.MapPath($"~/Uploads/Attachments/{ projId }/{ ticketAttachment.TicketId }/"), fileName));
                        ticketAttachment.FilePath = $"~/Uploads/Attachments/{ projId }/{ ticketAttachment.TicketId }/{ fileName }";
                        ticketAttachment.Created = DateTime.Now;
                        ticketAttachment.UserId = User.Identity.GetUserId();

                        db.TicketAttachments.Add(ticketAttachment);
                        db.SaveChanges();

                        var notification = new Notification
                        {
                            Created = DateTime.Now,
                            TicketId = ticket.Id,
                            SenderId = User.Identity.GetUserId(),
                            Subject = "New Attachment",
                            Body = $"Theres a new Attachment on one of your tickets! <br/>Ticket: { ticket.Title }<br/>Attachment: {ticket.Attachments.OrderByDescending(a => a.Created).FirstOrDefault().FileName }"
                        };
                        notificationHelper.TicketUpdateNotification(notification, ticket);
                    }
                    return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
                }

                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            } else
            {
                return RedirectToAction("Error", "Tickets", new { message = TicketsController.TicketError.NotAuthorizedToUpload });
            }
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,FilePath,FileUrl,Description,Created")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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
