using KillBug.Models;
using KillBug.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KillBug.Controllers
{
    public class ChartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET:Ticket Priority Data
        public JsonResult TicketPriorityChartData()
        {
            var data = new ChartData();
            data.KeyLabel = "# of Tickets";
            foreach (var priority in db.TicketPriorities.ToList())
            {
                var dataCount = db.Tickets.Where(t => t.TicketPriorityId == priority.Id).Count();
                data.Labels.Add(priority.Name);
                data.Data.Add(dataCount);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //GET:Ticket Status Data
        public JsonResult TicketStatusChartData()
        {
            var data = new ChartData();
            data.KeyLabel = "# of Tickets";
            foreach (var status in db.TicketStatus.ToList())
            {
                var dataCount = db.Tickets.Where(t => t.TicketStatusId == status.Id).Count();
                data.Labels.Add(status.Name);
                data.Data.Add(dataCount);
            }

            return Json(data);
        }
        //GET:Ticket Type Data
        public JsonResult TicketTypeChartData()
        {
            var data = new ChartData();
            data.KeyLabel = "# of Tickets";
            foreach (var type in db.TicketTypes.ToList())
            {
                var dataCount = db.Tickets.Where(t => t.TicketTypeId == type.Id).Count();
                data.Labels.Add(type.Name);
                data.Data.Add(dataCount);
            }
            return Json(data);
        }

    }
}