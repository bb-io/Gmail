using Apps.Gmail.Models.Requests;
using Apps.Gmail.Polling;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using Tests.Gmail.Base;

namespace Tests.Gmail
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task OnNewMailReceived_IsSuccess()
        {
            var polling = new EmailPolling(InvocationContext);
            var oldDate = DateTime.UtcNow.AddHours(5);
            var request = new PollingEventRequest<EmailsMemory>
            {
                Memory = new EmailsMemory
                {
                    EmailIds = []
                }
            };
            var serachRequest = new SearchEmailsRequest
            {
            };

            var response = polling.OnEmailsReceived(request, serachRequest);
            var serializedResponse = JsonConvert.SerializeObject(response.Result);
            Console.WriteLine(serializedResponse);

            Assert.IsNotNull(response);
        }
    }
}
