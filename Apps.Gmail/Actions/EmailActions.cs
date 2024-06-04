﻿using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Gmail.Actions
{
    [ActionList]
    public class EmailActions : GmailInvocable
    {
        public EmailActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Search emails", Description = "Search emails")]
        public async Task<SearchEmailsResponse> SearchEmails()
        {
            var emails = await Client.Users.Messages.List("me").ExecuteAsync();
            return new() { Emails = emails.Messages.Select(x => new EmailDto(x)).ToList() };
        }
    }
}
