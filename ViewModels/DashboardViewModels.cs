using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

    }
}