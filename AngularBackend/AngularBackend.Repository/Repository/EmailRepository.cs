using AngularBackend.Entities.ViewModels;
using AngularBackend.Repository.Interface;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Repository.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;
        public EmailRepository(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("panchalmeet1302@gmail.com");
                mail.To.Add(recipient);
                mail.Subject = "Please Reset Your Password!!";
                mail.IsBodyHtml = true;
                mail.Body = body;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("meetpanchal194@gmail.com", "ksdqxndnbbsofpyz");
                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }

            #region MimeMessage Technique to send an email
            //var emailMessage = new MimeMessage();
            //var From = _config["EmailCredentials:From"];
            //emailMessage.From.Add(new MailboxAddress("lets program", From));
            //emailMessage.To.Add(new MailboxAddress(mailViewModel.To, mailViewModel.To));
            //emailMessage.Subject = mailViewModel.Subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = string.Format(mailViewModel.Body)
            //};

            //using (var client = new SmtpClient())
            //{
            //    try
            //    {
            //        client.Connect(_config["EmailCredentials:SmtpServer"], 465, true);
            //        client.Authenticate("meetpanchal194@gmail.com", "ksdqxndnbbsofpyz");
            //        client.Send(emailMessage);
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //    finally
            //    {
            //        client.Disconnect(true);
            //        client.Dispose();
            //    }
            //}
            #endregion

        }
    }
}
