﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KillBug.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TicketPriority() { }
        public TicketPriority(string name)
        {
            Name = name;
        }
    }
}