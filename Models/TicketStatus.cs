﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TicketStatus() { }
        public TicketStatus(string name)
        {
            Name = name;
        }
    }
}