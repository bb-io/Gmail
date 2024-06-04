using Apps.Gmail.Clients;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Gmail.Invocables
{
    public class GmailInvocable : BaseInvocable
    {
        protected GoogleGmailClient Client { get; }

        public GmailInvocable(InvocationContext invocationContext) : base(invocationContext)
        {
            Client = new GoogleGmailClient(InvocationContext.AuthenticationCredentialsProviders);
        }
    }
}
