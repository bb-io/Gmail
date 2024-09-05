using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Gmail.Polling
{
    [PollingEventList]
    public class EmailPolling(InvocationContext invocationContext) : GmailInvocable(invocationContext)
    {
        [PollingEvent("On emails received", Description = "Triggered when new emails are received. You can optionally set a query to refine the search.")]
        public async Task<PollingEventResponse<EmailsMemory, SearchEmailsResponse>> OnEmailsReceived(PollingEventRequest<EmailsMemory> request, [PollingEventParameter] SearchEmailsRequest searchEmailsRequest)
        {
            if (request.Memory is null)
            {
                return new()
                {
                    FlyBird = false,
                    Memory = new()
                    {
                        EmailIds = new List<string>(){}
                    }
                };
            }

            var emailsRequest = Client.Users.Messages.List("me");
            if (!string.IsNullOrWhiteSpace(searchEmailsRequest.Query))
                emailsRequest.Q = searchEmailsRequest.Query;
            var emails = await emailsRequest.ExecuteAsync();
            var ids = emails.Messages.Select(x => x.Id);

            var newIds = ids.Where(x => !request.Memory.EmailIds.Contains(x));

            return new()
            {
                FlyBird = newIds.Any(),
                Result = new SearchEmailsResponse { EmailIds = newIds },
                Memory = new()
                {
                    EmailIds = request.Memory.EmailIds.Concat(newIds)
                }
            };
        }
    }
}
