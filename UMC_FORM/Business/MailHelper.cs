using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace UMC_FORM.Business
{
    public static class MailHelper
    {
        public static async Task SenMailOutlookAsync(string to, string body)
        {
            MailMessage mailMessage = new MailMessage();
            var maiAccount = Bet.Util.Config.GetValue("mail_account");
            var maiPass = Bet.Util.Config.GetValue("mail_password");
            SmtpClient smtpClient = new SmtpClient
            {
                EnableSsl = true,
                Host = "smtp.office365.com",
                Port = 587,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,

                TargetName = "STARTTLS/smtp.office365.com",
                Credentials = new NetworkCredential(maiAccount, maiPass)
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            mailMessage.From = new MailAddress(maiAccount);
            mailMessage.To.Add(to);
            mailMessage.Subject = Constant.SUBJECT;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mailMessage);
        }
        public static async Task SenMailOutlookAsync(List<string> to, string body, List<string> cc = null)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var maiAccount = Bet.Util.Config.GetValue("mail_account");
                var maiPass = Bet.Util.Config.GetValue("mail_password");
                SmtpClient smtpClient = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.office365.com",
                    Port = 587,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,

                    TargetName = "STARTTLS/smtp.office365.com",
                    Credentials = new NetworkCredential(maiAccount, maiPass)
                };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                mailMessage.From = new MailAddress(maiAccount,"System Testing");
                foreach (var item in to)
                {
                    mailMessage.To.Add(item);
                }
                if(cc != null && cc.Count > 0)
                {
                    foreach(var item in cc)
                    {
                        mailMessage.CC.Add(item);
                    }
                }
                
                mailMessage.Subject = Constant.SUBJECT;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }
           
        }

    }
}
