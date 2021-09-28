using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Services
{
    public class EmailMessage
    {
        public MailboxAddress To { get; set; }
         public string Subject { get; set; }
         public string Content { get; set; }
         public EmailMessage(string to, string content, string subject)
        {
            To =  MailboxAddress.Parse(to);
            Subject = subject;
            Content = content;
        }

    }
}
