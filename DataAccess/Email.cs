//using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TestProject.DataAccess
{
    public class Email
    {
        public int SendMail(string subject, string body, string tomails)
        {

            string Mailserver = "smtp.gmail.com";
            string port = "587";
            string ssl = "true";
            string loginemail = "shrivastavalovely6@gmail.com";
            string loginpassword = "Rcsjayanti1984@";
            string fromemail = "shrivastavalovely6@gmail.com";

            int result = 0;
            SmtpClient smtpClient = new SmtpClient();
            NetworkCredential basicCredential = new NetworkCredential(loginemail, loginpassword);
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress(fromemail);
            smtpClient.Host = Mailserver;
            smtpClient.Port = Convert.ToInt32(port);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;
            smtpClient.EnableSsl = Convert.ToBoolean(ssl);
            message.From = fromAddress;
            message.Subject = subject;
            //Set IsBodyHtml to true means you can send HTML email.
            message.IsBodyHtml = true;
            message.Body = body;
            message.To.Add(tomails);
           
            try
            {
                smtpClient.Send(message);
                result = 1;
            }
            catch (Exception ex)
            {
                //ex.Message;
                throw ex;
                //Error, could not send the message
                //Response.Write(ex.Message);
            }
            return result;
        }
    }
}

