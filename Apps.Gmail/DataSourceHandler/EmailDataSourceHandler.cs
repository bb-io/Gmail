using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Gmail.DataSourceHandler;

public class EmailDataSourceHandler(InvocationContext invocationContext) : GmailInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var emailsRequest = Client.Users.Messages.List("me");
        if (!string.IsNullOrWhiteSpace(context.SearchString))
        {
            emailsRequest.Q = context.SearchString;
        }

        var emails = await ExecuteWithErrorHandlingAsync(emailsRequest.ExecuteAsync);
        var foundEmails = emails.Messages.Select(x => GetFullEmail(x.Id));
        return foundEmails.Select(k => new DataSourceItem(k.Id, k.Subject));
    }

    private EmailDto GetFullEmail(string emailId)
    {
        var email = Client.Users.Messages.Get("me", emailId).Execute();
        return new(email);
    }
}
