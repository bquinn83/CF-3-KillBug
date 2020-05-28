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

        private void SaveNotification(Notification notification, string recipientId)
        {
            if (recipientId != null)
            {
                notification.RecipientId = recipientId;
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
        }
        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());

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
                    Body = $"You have been assigned as the developer for a ticket!<br/>Ticket: { newTicket.Title }<br/> Project: { newTicket.Project.Name }<br/> Priority: { newTicket.TicketPriority.Name }<br/> Type: { newTicket.TicketType.Name }"
                });
            }
            else if (unassigned)
            {
                db.Notifications.Add(new Notification
                {
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = "Ticket Assignment Removed",
                    Body = $"You have been removed from a ticket.<br/>Ticket: { newTicket.Title }<br/> Project: { newTicket.Project.Name }<br/> Priority: { newTicket.TicketPriority.Name }<br/> Type: { newTicket.TicketType.Name }"
                });
            }
            else if (reassigned)
            {
                //notify new developer
                db.Notifications.Add(new Notification
                {
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = "New Ticket Assignment",
                    Body = $"You have been assigned as the developer for a ticket!<br/>Ticket: { newTicket.Title }<br/> Project: { newTicket.Project.Name }<br/> Priority: { newTicket.TicketPriority.Name }<br/> Type: { newTicket.TicketType.Name }"
                });
                //notify old developer
                db.Notifications.Add(new Notification
                {
                    Created = DateTime.Now,
                    TicketId = newTicket.Id,
                    SenderId = HttpContext.Current.User.Identity.GetUserId(),
                    RecipientId = newTicket.DeveloperId,
                    Subject = "Ticket Assignment Removed",
                    Body = $"You have been removed from a ticket.<br/>Ticket: { newTicket.Title }<br/> Project: { newTicket.Project.Name }<br/> Priority: { newTicket.TicketPriority.Name }<br/> Type: { newTicket.TicketType.Name }"
                });
            }

            db.SaveChanges();
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
        public void NewTicketNotification(Notification notification, Ticket ticket)
        {
            SaveNotification(notification, ticket.Project.ProjectManagerId);
        }
        public void NewProjectNotification(Project project)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            Notification newNotification = new Notification
            {
                Created = DateTime.Now,
                SenderId = user.Id,
                Subject = "Project Manager Assignment",
                Body = $"You have been assigned as a Project Manager to a new Project! <br/>Project: { project.Name }<br/>By: { user.FullNamePosition }"
            };
            SaveNotification(newNotification, project.ProjectManagerId);
        }
        public void ProjectAssignmentNotification(int projectId, string recipientId)
        {
            var proj = db.Projects.Find(projectId);

            Notification notification = new Notification
            {
                Created = DateTime.Now,
                ProjectId = projectId,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "New Project Assignment",
                Body = $"You have been assigned to a new project!<br/>Project: { proj.Name }"
            };
            SaveNotification(notification, recipientId);
        }
        public void RemovedProjectNotification(int projectId, string recipientId)
        {
            var proj = db.Projects.Find(projectId);

            Notification notification = new Notification
            {
                Created = DateTime.Now,
                ProjectId = projectId,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "Project Removed",
                Body = $"You have been removed from a project.<br/>Project: { proj.Name }"
            };
            SaveNotification(notification, recipientId);
        }
        public void NewProjectManagerNotification(int projectId, string newProjectManagerId)
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            Project project = db.Projects.Find(projectId);
            //Notify OLD PM
            Notification oldPMnote = new Notification
            {
                Created = DateTime.Now,
                ProjectId = project.Id,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "Project Manager Removed",
                Body = $"You are no longer the Project Manager of a project.<br/>Project: { project.Name }<br/>By: { user.FullNamePosition }"
            };
            SaveNotification(oldPMnote, project.ProjectManagerId);
            //Notify NEW PM
            Notification newPMnote = new Notification
            {
                Created = DateTime.Now,
                ProjectId = project.Id,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "Project Manager Assignment",
                Body = $"You have been assigned as a Project Manager to a new Project! <br/>Project: { project.Name }<br/>By: { user.FullNamePosition }"
            };
            SaveNotification(newPMnote, newProjectManagerId);
            //Notify Users
            Notification note = new Notification
            {
                Created = DateTime.Now,
                ProjectId = project.Id,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                Subject = "New Project Manager",
                Body = $"You have a new Project Manager for one of your projects!<br/>Project: { project.Name }<br/>Manager: { db.Users.Find(newProjectManagerId).FullName }"
            };
            foreach (var projUser in project.Users)
            {
                SaveNotification(note, projUser.Id);
            }
        }
    }
}