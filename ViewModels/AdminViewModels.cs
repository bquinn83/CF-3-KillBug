using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KillBug.Models;

namespace KillBug.ViewModels
{
    public class UserRoleVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
    public class TicketSettingsVM
    {
        public List<TicketType> Types = new List<TicketType>();
        public List<TicketStatus> Status = new List<TicketStatus>();
        public List<TicketPriority> Priorities = new List<TicketPriority>();

    }
}