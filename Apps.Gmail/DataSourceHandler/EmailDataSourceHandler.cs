using Apps.Gmail.Dtos;
using Apps.Gmail.Invocables;
using Apps.Gmail.Models.Requests;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Gmail.DataSourceHandler
{
    public class EmailDataSourceHandler : GmailInvocable, IAsyncDataSourceHandler
    {
        public EmailDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var emailsRequest = Client.Users.Messages.List("me");
            if(!string.IsNullOrWhiteSpace(context.SearchString))
                emailsRequest.Q = context.SearchString;
            var emails = await emailsRequest.ExecuteAsync();
            var foundEmails = emails.Messages.Take(10).Select(x => GetFullEmail(x.Id));
            return foundEmails.ToDictionary(k => k.Id, v => v.Subject);
        }

        private EmailDto GetFullEmail(string emailId)
        {
            var email = Client.Users.Messages.Get("me", emailId).Execute();
            return new(email);
        }
    }
}
