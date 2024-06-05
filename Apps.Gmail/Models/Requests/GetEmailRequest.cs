using Apps.Gmail.DataSourceHandler;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Gmail.Models.Requests
{
    public class GetEmailRequest
    {
        [Display("Email ID")]
        [DataSource(typeof(EmailDataSourceHandler))]
        public string EmailId { get; set; }
    }
}
