using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Gmail.Actions
{
    [ActionList]
    public class EmailActions : GmailInvocable
    {
        public EmailActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Search emails", Description = "Search emails")]
        public async Task<SearchEmailsResponse> SearchEmails([ActionParameter] SearchEmailsRequest searchEmailsRequest)
        {
            var emailsRequest = Client.Users.Messages.List(searchEmailsRequest.Email);
            var emails = await emailsRequest.ExecuteAsync();


            var emailTest = Client.Users.Messages.Get("me", emails.Messages.First().Id).Execute();

            return new() { Emails = emails.Messages.Select(x => new EmailDto(x)).ToList() };
        }

        [Action("Get email", Description = "Get email")]
        public async Task<EmailDto> GetEmail([ActionParameter] GetEmailRequest getEmailRequest)
        {
            var email = await Client.Users.Messages.Get("me", getEmailRequest.EmailId).ExecuteAsync();
            return new(email);
        }
    }
}
