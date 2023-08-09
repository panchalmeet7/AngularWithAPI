using AngularBackend.Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Common.CommonMethods
{
    public class CommonMethods
    {
        private readonly IConfiguration _config;
        public CommonMethods(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Token For ResetPassword
        public static string CreateTokenForResetPassword()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }
        #endregion

        #region Create JWT Token
        public static string CreateToken(User user) //JWT Token Contains 3 things => Header, Payload & Signature
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("CI_PlatForm_Secreat_Key_Is_Demo_With256Bits");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        #endregion

        #region Send Email Through SMTP Client
        public static void SendEmail(string recipient, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("panchalmeet1302@gmail.com");
                mail.To.Add(recipient);
                mail.Subject = subject;
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

            #region MimeMessage Technique
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
        #endregion
    }
}
