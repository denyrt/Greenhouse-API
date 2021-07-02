using Greenhouse.Models.Options;
using Greenhouse.Services.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Greenhouse.Services
{
    public class SendGridEmailService : IEmailService
    {
        public SendGridOptions Options { get; }

        public SendGridEmailService(IOptions<SendGridOptions> sendGridOptions)
        {
            if (sendGridOptions == null)
                throw new ArgumentNullException(nameof(sendGridOptions), "");

            Options = sendGridOptions.Value;
        }

        public async Task<bool> SendMessageAsync(string email,
                                                 string subject,
                                                 string content,
                                                 string htmlContent = null)
        {
            return await Execute(Options.SendGridKey, email, subject, content, htmlContent);
        }

        public async Task<bool> Execute(string apiKey, string email, string subject, string content, string htmlContent = null)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Options.SendGridEmail, Options.SendGridUser);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode; // You also can check status code and add message in some logger. 
        }
    }
}
