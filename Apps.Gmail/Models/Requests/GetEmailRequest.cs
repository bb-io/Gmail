using Blackbird.Applications.Sdk.Common;

namespace Apps.Gmail.Models.Requests;

public class GetEmailRequest
{
    [Display("Email ID")]
    public string EmailId { get; set; }
}
