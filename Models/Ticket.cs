using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class Ticket
    {
        #region IDS
        public int Id { get; set; }
        public int ProjectId { get; set; }
        [DisplayName("Type")]
        public int TicketTypeId { get; set; }
        [DisplayName("Status")]
        public int TicketStatusId { get; set; }
        [DisplayName("Priority")]
        public int TicketPriorityId { get; set; }
        [DisplayName("Submitter")]
        public string SubmitterId { get; set; }
        [DisplayName("Developer")]
        public string DeveloperId { get; set; }
        #endregion

        #region DATA
        public string Title { get; set; }
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsArchived { get; set; }
        #endregion

        #region NAVIGATION
        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual ApplicationUser Submitter { get; set; }
        public virtual ApplicationUser Developer { get; set; }
        public virtual ICollection<TicketAttachment> Attachments { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }
        #endregion

        public Ticket()
        {
            Attachments = new HashSet<TicketAttachment>();
            Comments = new HashSet<TicketComment>();
            History = new HashSet<TicketHistory>();
        }
    }
}