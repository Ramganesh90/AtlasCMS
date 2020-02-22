using Atlas.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace Atlas.DataAccess
{
    public class MailUtil
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _ssl;

        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string RecipientCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public MailUtil()
        {
            _host = ConfigurationManager.AppSettings["MailServer"];
            _port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            _user = ConfigurationManager.AppSettings["MailAuthUser"];
            _pass = ConfigurationManager.AppSettings["MailAuthPass"];
            _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
            Sender = Convert.ToString(ConfigurationManager.AppSettings["EmailFromAddress"]);
            Recipient = Convert.ToString(ConfigurationManager.AppSettings["SendPunchListRecipients"]);

        }
        public MailUtil(string subject, string body)
        {

            var mailConfig = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
            _host = mailConfig.Network.Host;
            _port = mailConfig.Network.Port;
            _user = mailConfig.Network.UserName;
            _pass = mailConfig.Network.Password;
            _ssl = mailConfig.Network.EnableSsl;
            Sender =mailConfig.Network.UserName;
            Recipient = Convert.ToString(ConfigurationManager.AppSettings["SendPunchListRecipients"]);
            Subject = subject;
            Body = body;
        }

        public void Send()
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(Sender);
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = true;
                string[] multiRecipients = Recipient.Split(';');
                foreach (var mailId in multiRecipients)
                {
                    message.To.Add(new MailAddress(mailId));
                }
                var smtp = new SmtpClient(_host, _port);

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.EnableSsl = _ssl;
                }

                smtp.Send(message);
                message.Dispose();
                smtp.Dispose();
            }

            catch (Exception ex)
            {
                Logger.SaveErr(ex);
            }
        }
    }
}