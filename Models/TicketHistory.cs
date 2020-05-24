using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class TicketHistory
    {
        #region IDS
        public int Id { get; set; }
        public int TicketId { get; set; }
        [DisplayName("Edited By")]
        public string UserId { get; set; }
        #endregion

        #region PROPERTIES
        public string Property { get; set; }
        [DisplayName("Old Value")]
        public string OldValue { get; set; }
        [DisplayName("New Value")]
        public string NewValue { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [DisplayName("Edited On")]
        public DateTime ChangedOn { get; set; }
        #endregion

        #region DB Navigation
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
        #endregion
    }
}