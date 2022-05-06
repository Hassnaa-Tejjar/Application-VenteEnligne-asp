using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace aspprojet.Models
{
    public class Gmail
    {

        public string To { get; set; }
        public string Subject{ get; set; }
        public string Body { get; set; }
       /* public void sendMail()
        {
            MailMessage m = new MailMessage("hassnaatejjar03@gmail.com", To);
            m.Subject = Subject;
            m.Body = Body;
            m.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("hassnaatejjar03@gmail.com","hlhsfe987654321");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(m);
        }*/
    }
}