using leave_management.Contracts;
using leave_management.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using leave_management.Utilities;
using leave_management.Data;

namespace leave_management.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ApplicationDbContext _db;
        public EmailRepository( EmailConfiguration emailConfig, ApplicationDbContext db)
        {
            _emailConfig = emailConfig;
            _db = db;
        }
        public void SendEmail(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
           
            try
            {
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "<h2>Core Leave Management</h2>";
                bodyBuilder.TextBody = message.Content;

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
                emailMessage.To.Add(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

                return emailMessage;
            }
            catch (Exception ex)
            {
                ErrorLogger err = new ErrorLogger();
                var logError = err.logError(ex);
                _db.ErrorLogs.AddAsync(logError);
                _db.SaveChangesAsync();
                throw ex;
            }
        }
        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                client.Send(mailMessage);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
