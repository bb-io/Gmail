using Apps.Gmail.Clients;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Gmail.Invocables;

public class GmailInvocable : BaseInvocable
{
    protected GoogleGmailClient Client { get; }

    public GmailInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new GoogleGmailClient(InvocationContext.AuthenticationCredentialsProviders);
    }

    protected async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(ex.Message, ex);
        }
    }
}
