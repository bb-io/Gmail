using Apps.Gmail.DataSourceHandler;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Gmail.Base;

namespace Tests.Gmail;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task EmailDataHandler_IsSuccess()
    {
        var handler = new EmailDataSourceHandler(InvocationContext);
        var context = new DataSourceContext
        {
            SearchString = ""
        };
        var result = await handler.GetDataAsync(context, CancellationToken.None);

        foreach (var email in result)
        {
            Console.WriteLine($"Email ID: {email.Value}, Subject: {email.DisplayName}");
        }

        Assert.IsNotNull(result);
    }
}
