using KillBug.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KillBug.ViewModels
{
    public class DashboardViewModel
    {
        public double TeamCount { get; set; }
        public double PMCount { get; set; }
        public double DevCount { get; set; }
        public double SubCount { get; set; }
        public int ProjectsCount { get; set; }
        public int TicketsCount { get; set; }
        public int NewTicketsCount { get; set; }
        public int CommentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int HistoryCount { get; set; }
    }
    public class MainDashViewModel
    {

    }
    public class ProjectsDashViewModel
    {
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Project Manager")]
        public SelectList ProjectManagerId { get; set; }

        public List<Project> MyProjects { get; set; }
    }

    public class TicketsDashViewModel
    {
        [DisplayName("Projects")]
        public SelectList ProjectId {get; set;}
        
        [DisplayName("Ticket Types")]
        public SelectList TicketTypeId {get; set;}
        
        [DisplayName("Ticket Priority")]
        public SelectList TicketPriorityId {get; set;}
        
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        public List<Ticket> MyTickets { get; set; }

        public JsonResult PriorityData { get; set; }
        public object JsonData { get; internal set; }
    }
}