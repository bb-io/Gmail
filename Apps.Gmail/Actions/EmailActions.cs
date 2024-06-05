using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using RestSharp;
using System;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace Apps.Gmail.Actions
{
    [ActionList]
    public class EmailActions : GmailInvocable
    {
        public EmailActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Search emails", Description = "Search emails. Specify query to fetch emails or you will get last 10 emails")]
        public async Task<SearchEmailsResponse> SearchEmails([ActionParameter] SearchEmailsRequest searchEmailsRequest)
        {
            var emailsRequest = Client.Users.Messages.List("me");
            if (!string.IsNullOrWhiteSpace(searchEmailsRequest.Query))
                emailsRequest.Q = searchEmailsRequest.Query;
            var emails = await emailsRequest.ExecuteAsync();
            var foundEmails = emails.Messages.Take(10).Select(x => GetEmail(new GetEmailRequest() { EmailId = x.Id }).Result).ToList();
            return new() { Emails = foundEmails };
        }

        [Action("Get email", Description = "Get email")]
        public async Task<EmailDto> GetEmail([ActionParameter] GetEmailRequest getEmailRequest)
        {
            var email = await Client.Users.Messages.Get("me", getEmailRequest.EmailId).ExecuteAsync();
            return new(email);
        }

        [Action("Send email", Description = "Send email")]
        public async Task<EmailDto> SendEmail([ActionParameter] SendEmailRequest getEmailRequest)
        {
            var mailMessage = new MailMessage("defaultmakarenko@gmail.com", getEmailRequest.To, getEmailRequest.Subject, getEmailRequest.Body);

            var mimeMessage = MimeMessage.CreateFromMailMessage(mailMessage);

            using var mailMessageStream = new MemoryStream();
            mimeMessage.WriteTo(mailMessageStream);
            var base64Content = Convert.ToBase64String(mailMessageStream.ToArray());

            var message = new Message()
            {
                Raw = base64Content
            };
            var email = await Client.Users.Messages.Send(message, "me").ExecuteAsync();
            return new(email);
        }
    }
}
