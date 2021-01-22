using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace KillBug.Services
{
    public class EmailSender
    {
        public string MailGunUrl { get; set; }
        public string MailGunKey { get; set; }
        public string FromAddress { get; set; }
        public EmailSender()
        {
            MailGunUrl = WebConfigurationManager.AppSettings["MailGunUrl"];
            MailGunKey = WebConfigurationManager.AppSettings["MailGunKey"];
            FromAddress = WebConfigurationManager.AppSettings["MailFromAddress"];
        }

        public Task Execute(string subject, string template, string email, IMailTemplate templateVars)
        {
            RestClient client = new RestClient();

            client.BaseUrl = new Uri(MailGunUrl);
            client.Authenticator = new HttpBasicAuthenticator("api", MailGunKey);

            RestRequest request = new RestRequest();

            request.AddParameter("domain", "brianquinn.info", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.Method = Method.POST;

            request.AddParameter("from", FromAddress);

            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("template", template);
            request.AddParameter("h:X-Mailgun-Variables", JsonConvert.SerializeObject(templateVars));

            return client.ExecuteAsync(request);
        }
    }
}