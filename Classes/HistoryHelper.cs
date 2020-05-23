using KillBug.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace KillBug.Classes
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void CreateHistory(Ticket oldTicket, Ticket newTicket)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();

            if (oldTicket.Title != newTicket.Title)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Title",
                    OldValue = oldTicket.Title,
                    NewValue = newTicket.Title,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            if (oldTicket.Description != newTicket.Description)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Ticket Type",
                    OldValue = oldTicket.TicketTypeId.ToString(),
                    NewValue = newTicket.TicketTypeId.ToString(),
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Ticket Priority",
                    OldValue = oldTicket.TicketPriorityId.ToString(),
                    NewValue = newTicket.TicketPriorityId.ToString(),
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Ticket Status",
                    OldValue = oldTicket.TicketStatusId.ToString(),
                    NewValue = newTicket.TicketStatusId.ToString(),
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            if (oldTicket.DeveloperId != newTicket.DeveloperId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    ChangedOn = (DateTime)newTicket.Updated,
                    UserId = userId,
                    Property = "Developer",
                    OldValue = oldTicket.DeveloperId,
                    NewValue = newTicket.DeveloperId,
                    TicketId = newTicket.Id
                };
                db.TicketHistories.Add(newHistoryRecord);
            }
            db.SaveChanges();
        }

        public string DisplayValue(string property, string value)
        {
            string displayValue;
            if (value == null)
            {
                return "Un-Assigned";
            }
            switch (property)
            {
                case "Developer":
                    displayValue = db.Users.Find(value).FullName;
                    break;
                case "Ticket Priority":
                    displayValue = db.TicketPriorities.Find(Convert.ToInt32(value)).Name;
                    break;
                case "Ticket Type":
                    displayValue = db.TicketTypes.Find(Convert.ToInt32(value)).Name;
                    break;
                case "Ticket Status":
                    displayValue = db.TicketStatus.Find(Convert.ToInt32(value)).Name;
                    break;
                default:
                    displayValue = value;
                    break;
            }
            return displayValue;
        }
        public static string Display(string property, string value)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            string displayValue;
            if (value == null)
            {
                return "Un-Assigned";
            }
            switch (property)
            {
                case "Developer":
                    displayValue = db.Users.Find(value).FullName;
                    break;
                case "Ticket Priority":
                    displayValue = db.TicketPriorities.Find(Convert.ToInt32(value)).Name;
                    break;
                case "Ticket Type":
                    displayValue = db.TicketTypes.Find(Convert.ToInt32(value)).Name;
                    break;
                case "Ticket Status":
                    displayValue = db.TicketStatus.Find(Convert.ToInt32(value)).Name;
                    break;
                default:
                    displayValue = value;
                    break;
            }
            return displayValue;
        }

        public static List<TicketHistory> ListMyHistory()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            List<TicketHistory> ticketHistories = new List<TicketHistory>();

            switch (user.UserRole())
            {
                case "Admin":
                    ticketHistories = db.TicketHistories.ToList();
                    break;
                case "Project Manager":
                    ticketHistories = db.TicketHistories.Include(t => t.Ticket).Include(t => t.User).Where(t => t.Ticket.Project.ProjectManagerId == user.Id).ToList();
                    //NOT TICKETS YOUR ASSIGNED TO (BECAUSE THATS NOT A THING)
                    break;
                case "Developer":
                    ticketHistories = db.TicketHistories.Include(t => t.Ticket).Include(t => t.User).Where(t => t.Ticket.DeveloperId == user.Id).ToList();
                    break;
                case "Submitter":
                    ticketHistories = db.TicketHistories.Include(t => t.Ticket).Include(t => t.User).Where(t => t.Ticket.SubmitterId == user.Id).ToList();
                    break;
            }

            return ticketHistories;
        }
    }
}