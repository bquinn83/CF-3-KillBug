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

        public static List<Notification> GetUnreadNotifications()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = HttpContext.Current.User.Identity.GetUserId();

            if (userId == null)
                return new List<Notification>();

            return db.Notifications.Where(t => t.RecipientId == userId && !t.IsRead).ToList();
        }
        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

            //Manage a Developer assignment notification
            //EXTRA CREDIT: See if there is a developer being assigned or re-assigned
            if (oldTicket.DeveloperId != newTicket.DeveloperId)
            {
                DeveloperAssignmentNotifications(oldTicket, newTicket);
            }
            //Manage some other general change notification
            Notification newNotification = new Notification
            {
                Created = DateTime.Now,
                TicketId = newTicket.Id,
                SenderId = user.Id,
                Subject = "Ticket Updated",
                Body = $"One of your tickets has been updated! <br/>Ticket: { newTicket.Title }<br/>By: { user.FullNamePosition }"
            };
            TicketUpdateNotification(newNotification, newTicket);
        }
        private void DeveloperAssignmentNotifications(Ticket oldTicket, Ticket newTicket)
        {
            bool assigned = oldTicket.DeveloperId == null && newTicket.DeveloperId != null;
            bool unassigned = oldTicket.DeveloperId != null && newTicket.DeveloperId == null;
            bool reassigned = !assigned && !unassigned && oldTicket.DeveloperId != newTicket.DeveloperId;

            if (assigned)
            {
                db.Notifications.Add(new Notification
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
        private void SaveNotification(Notification notification, string recipientId)
        {
            if (recipientId != null)
            {
                notification.RecipientId = recipientId;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
        }
        public void TicketUpdateNotification(Notification newNotification, Ticket ticket)
        {
            ApplicationUser user = db.Users.Find(newNotification.SenderId);

            switch (user.UserRole())
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
                case "Developer":
                    SaveNotification(newNotification, ticket.Project.ProjectManagerId);
                    SaveNotification(newNotification, ticket.SubmitterId);
                    break;
                case "Submitter":
                    SaveNotification(newNotification, ticket.Project.ProjectManagerId);
                    SaveNotification(newNotification, ticket.DeveloperId);
                    break;
            }
        }
        public void RoleChangeNotification(string userId, string roleName)
        {
            var notification = new Notification
            {
                Created = DateTime.Now,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "New Role!",
                Body = $"Your role has been changed to { roleName }!",
            };

            SaveNotification(notification, userId);
        }

        public void ManagerUpdateNotification()
        {

        }
    }
}