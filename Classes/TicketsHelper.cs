using KillBug.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KillBug.Classes
{
    public class TicketComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            throw new NotImplementedException();
        }
    }
    public class TicketsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper roleHelper = new UserRolesHelper();
        ProjectsHelper projectHelper = new ProjectsHelper();

        public ICollection<ApplicationUser> AssignableDevelopers(int projectId)
        {
            var developers = roleHelper.UsersInRole("Developer");
            var available = new List<ApplicationUser>();
            foreach (var user in developers)
            {
                if (projectHelper.IsUserOnProject(user.Id, projectId))
                {
                    available.Add(user);
                }
            };
            return available;
        }

        public List<Ticket> ListMyTickets()
        {
            var myTickets = new List<Ticket>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (myRole)
            {
                case "Admin":
                    myTickets.AddRange(db.Tickets);
                    break;
                case "Project Manager":
                    myTickets.AddRange(user.Projects.Where(p => p.IsArchived == false).SelectMany(p => p.Tickets));
                    myTickets.AddRange(db.Projects.Where(p => p.ProjectManagerId == userId).SelectMany(p => p.Tickets));
                    break;
                case "Developer":
                    myTickets.AddRange(db.Tickets.Where(t => t.IsArchived == false).Where(testc => testc.DeveloperId == userId));
                    break;
                case "Submitter":
                    myTickets.AddRange(db.Tickets.Where(t => t.IsArchived == false).Where(t => t.SubmitterId == userId));
                    break;
            }
            return myTickets;
        }
    }
}