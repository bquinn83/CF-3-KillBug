using KillBug.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KillBug.ViewModels
{
    public class TicketsIndexViewModel
    {
        public Ticket Ticket { get; set; }
        //public TicketModel Ticket { get; set; }
        public IEnumerable<SelectListItem> TicketStatus { get; set; }
        public int TicketStatusId { get; set; }
    }

    public class EditTicketViewModel
    {

    }
}