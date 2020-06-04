using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TicketType() { }
        public TicketType(string type)
        {
            Name = type;
        }
        public TicketType(string type, string description)
        {
            Name = type;
            Description = description;
        }
    }
}