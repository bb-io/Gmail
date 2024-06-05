using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System;

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
    }
}
