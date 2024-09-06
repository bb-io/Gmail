using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using RestSharp;
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Apps.Gmail.Actions
{
    [ActionList]
    public class EmailActions : GmailInvocable
    {
        private readonly IFileManagementClient _fileManagementClient;

        public EmailActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(invocationContext)
        {
            _fileManagementClient = fileManagementClient;
        }

        [Action("Search emails", Description = "Returns a list of IDs which can be used in conjunction with 'Get email'. Add an optional query to narrow your search. ")]
        public async Task<SearchEmailsResponse> SearchEmails([ActionParameter] SearchEmailsRequest searchEmailsRequest)
        {
            var emailsRequest = Client.Users.Messages.List("me");
            emailsRequest.LabelIds = new List<string>() { "INBOX" };
            if (!string.IsNullOrWhiteSpace(searchEmailsRequest.Query))
                emailsRequest.Q = searchEmailsRequest.Query;
            var emails = await emailsRequest.ExecuteAsync();
            return new() { EmailIds = emails.Messages.Select(x => x.Id) };
        }

        [Action("Get email", Description = "Returns email metadata, message and all attachments")]
        public async Task<EmailDto> GetEmail([ActionParameter] GetEmailRequest getEmailRequest)
        {
            var email = await Client.Users.Messages.Get("me", getEmailRequest.EmailId).ExecuteAsync();
            var attachments = new List<FileReference>();

            if (email.Payload.Parts != null)
            {
                var attachmentParts = email.Payload.Parts.Where(x => !string.IsNullOrEmpty(x.Filename));
                foreach (var part in attachmentParts)
                {
                    var att = await Client.Users.Messages.Attachments.Get("me", getEmailRequest.EmailId, part.Body.AttachmentId).ExecuteAsync();
                    var base64EncodedBytes = Convert.FromBase64String(att.Data.Replace("-", "+").Replace("_", "/"));
                    var fileBytes = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(base64EncodedBytes));
                    var file = await _fileManagementClient.UploadAsync(new MemoryStream(fileBytes), part.MimeType, part.Filename);
                    attachments.Add(file);
                }
            }
            
            return new EmailDto(email) { Attachments = attachments };
        }

        [Action("Send email", Description = "Sends an email, including attachments")]
        public async Task<EmailDto> SendEmail([ActionParameter] SendEmailRequest sendEmailRequest)
        {
            var myProfile = await Client.Users.GetProfile("me").ExecuteAsync();
            var mailMessage = new MailMessage(myProfile.EmailAddress, sendEmailRequest.To, sendEmailRequest.Subject, sendEmailRequest.Message);

            if(sendEmailRequest.CC != null)
            {
                foreach(var ccEmail in sendEmailRequest.CC)
                {
                    mailMessage.CC.Add(ccEmail);
                }        
            }

            if (sendEmailRequest.Attachments != null)
            {
                foreach (var file in sendEmailRequest.Attachments)
                {
                    var fileStream = await _fileManagementClient.DownloadAsync(file);
                    mailMessage.Attachments.Add(new Attachment(fileStream, file.Name));
                }
            }

            var mimeMessage = MimeMessage.CreateFromMailMessage(mailMessage);

            using var mailMessageStream = new MemoryStream();
            mimeMessage.WriteTo(mailMessageStream);
            var base64Content = Convert.ToBase64String(mailMessageStream.ToArray());

            var message = new Message()
            {
                Raw = base64Content
            };
            var email = await Client.Users.Messages.Send(message, "me").ExecuteAsync();

            return await GetEmail(new GetEmailRequest { EmailId = email.Id});
        }
    }
}
