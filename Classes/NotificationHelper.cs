using KillBug.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KillBug.Classes
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();

        public static List<TicketNotification> GetUnreadNotifications()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = HttpContext.Current.User.Identity.GetUserId();

            if (userId == null)
                return new List<TicketNotification>();

            return db.TicketNotifications.Where(t => t.RecipientId == userId && !t.IsRead).ToList();
        }

        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            //Manage a Developer assignment notification
            //EXTRA CREDIT: See if there is a developer being assigned or re-assigned
            ManageAssignmentNotifications(oldTicket, newTicket);
            //Manage some other general change notification

            //GenerateTicketChangeNotification(oldTicket, newTicket);

        }

        private void ManageAssignmentNotifications(Ticket oldTicket, Ticket newTicket)
        {
            bool assigned = oldTicket.DeveloperId == null && newTicket.DeveloperId != null;
            bool unassigned = oldTicket.DeveloperId != null && newTicket.DeveloperId == null;
            bool reassigned = !assigned && !unassigned && oldTicket.DeveloperId != newTicket.DeveloperId;

            if (assigned)
            {
                db.TicketNotifications.Add(new TicketNotification
                {
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = "New Ticket Assignment",
                    Body = $"Ticket: { newTicket.Title }<br/> Project: { newTicket.Project.Name }<br/> Priority: { newTicket.TicketPriority.Name }<br/> Type: { newTicket.TicketType.Name }"
                });
            }

            db.SaveChanges();
        }

        private void GenerateTicketChangeNotification(Ticket oldTicket, Ticket newTicket)
        {
            throw new NotImplementedException();
        }

        public void AttachmentNotification(Ticket ticket)
        {
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            string userRole = rolesHelper.ListUserRoles(user.Id).FirstOrDefault();

            TicketNotification newNotification = new TicketNotification
            {
                Created = DateTime.Now,
                TicketId = ticket.Id,
                SenderId = user.Id,
                Subject = "New Attachment",
                Body = $"Theres a new Attachment on one of your tickets! <br/>Ticket: { ticket.Title }<br/>Attachment: {ticket.Attachments.OrderByDescending(a => a.Created).FirstOrDefault().FileName }"
            };

            switch (userRole)
            {
                case "Admin":
                    SaveNotification(newNotification, ticket.Project.ProjectManagerId);
                    SaveNotification(newNotification, ticket.DeveloperId);
                    SaveNotification(newNotification, ticket.SubmitterId);
                    break;
                case "Project Manager":
                    SaveNotification(newNotification, ticket.DeveloperId);
                    SaveNotification(newNotification, ticket.SubmitterId);
                    break;
                case "Submitter":
                    SaveNotification(newNotification, ticket.Project.ProjectManagerId);
                    SaveNotification(newNotification, ticket.DeveloperId);
                    break;
            }
        }

        private void SaveNotification(TicketNotification notification, string recipientId)
        {
            if (recipientId != null)
            {
                notification.RecipientId = recipientId;
                db.TicketNotifications.Add(notification);
                db.SaveChanges();
            }
        }
    }
}