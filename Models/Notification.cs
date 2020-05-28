using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace KillBug.Models
{
    public class Notification
    {
        #region IDS
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        #endregion

        #region DATA
        public bool IsRead { get; set; }
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy hh:mm tt}")]
        public DateTime Created { get; set; }
        #endregion

        #region NAVIGATION
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        //NOT IMPLEMENTED:
        public int? TicketId { get; set; }
        public int? ProjectId { get; set; }
        //Why are the above Id's here??? For future feature, having a notification about a Project/Ticket be able to link to the Project/Ticket
        #endregion
    }
}