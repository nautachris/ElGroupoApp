using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Services
{
    public interface IEmailSender
    {
        Task<EmailResponse> SendEmailAsync(string email, string subject, string message);
    }

    public class EmailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
