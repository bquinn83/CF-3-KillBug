using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KillBug.Models
{
    public class EmailModel
    {
        [Required, Display(Name = "Name")]
        public string FromName { get; set; }

        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }

        [Required]
        [EmailAddress]
        public string ToEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required, AllowHtml, Display(Name = "Message")]
        public string Body { get; set; }

        public EmailModel() {}

        public async Task Launch() 
        {
            MailAddress From = new MailAddress(FromEmail, FromName);
            MailAddress To = new MailAddress(ToEmail);
            var email = new MailMessage(From, To)
            {
                Subject = Subject,
                Body = Body,
                IsBodyHtml = true
            };

            var svc = new EmailService();
            await svc.SendAsync(email);
        }
    }
}