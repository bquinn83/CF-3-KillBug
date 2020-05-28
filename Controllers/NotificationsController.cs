using KillBug.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KillBug.Controllers
{
    public class NotificationsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public void HasBeenRead(int notificationID)
        {
            db.Notifications.Find(notificationID).IsRead = true;
            db.SaveChanges();
        }
    }
}