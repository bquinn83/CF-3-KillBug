using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KillBug.Models;

namespace KillBug.ViewModels
{
    public class DeveloperAssignmentsViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Projects { get; set; }
    }
}