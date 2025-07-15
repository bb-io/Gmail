using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Apps.Gmail.Models.Responses;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Gmail.Polling;

[PollingEventList]
public class EmailPolling(InvocationContext invocationContext) : GmailInvocable(invocationContext)
{
    [PollingEvent("On emails received", Description = "Triggered when new emails are received. You can optionally set a query to refine the search.")]
    public async Task<PollingEventResponse<EmailsMemory, SearchEmailsResponse>> OnEmailsReceived(PollingEventRequest<EmailsMemory> request, [PollingEventParameter] SearchEmailsRequest searchEmailsRequest)
    {
        var emailsRequest = Client.Users.Messages.List("me");
        emailsRequest.LabelIds = new List<string> { "INBOX" };
        emailsRequest.MaxResults = 100;

        DateTime lastTime = request.Memory?.LastTimeInteraction ?? DateTime.UtcNow.AddDays(-1);
        long unixTimeSeconds = new DateTimeOffset(lastTime.ToUniversalTime()).ToUnixTimeSeconds();
        string timeQuery = $"after:{unixTimeSeconds}";
        if (!string.IsNullOrWhiteSpace(searchEmailsRequest.Query))
            emailsRequest.Q = $"{timeQuery} {searchEmailsRequest.Query}";
        else
            emailsRequest.Q = timeQuery;

        List<string> allIds = new List<string>();
        string pageToken = null;

        do
        {
            emailsRequest.PageToken = pageToken;
            var emails = await emailsRequest.ExecuteAsync();
            if (emails.Messages != null)
            {
                allIds.AddRange(emails.Messages.Select(x => x.Id));
            }
            pageToken = emails.NextPageToken;
        } while (!string.IsNullOrEmpty(pageToken) && allIds.Count < 500);

        var ids = allIds.AsEnumerable();

        if (request.Memory is null || !ids.Any())
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastTimeInteraction = DateTime.UtcNow
                }
            };
        }

        return new()
        {
            FlyBird = true,
            Result = new SearchEmailsResponse { EmailIds = ids },
            Memory = new()
            {
                LastTimeInteraction = DateTime.UtcNow
            }
        };
    }
}
