using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class Project
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }

        [DisplayName("Project Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectManagerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime? Updated { get; set; }
        public bool IsArchived { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        [NotMapped]
        public ApplicationUser ProjectManager { get { return db.Users.FirstOrDefault(u => u.Id == ProjectManagerId); } }

        public Project()
        {
            Users = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
        }
    }
}