using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KillBug.Models
{
    public class TicketNotification
    {
        #region IDS
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        #endregion

        #region DATA
        public bool IsRead { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yy hh:mm tt}")]
        public DateTime Created { get; set; }
        #endregion

        #region NAVIGATION
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        #endregion
    }
}