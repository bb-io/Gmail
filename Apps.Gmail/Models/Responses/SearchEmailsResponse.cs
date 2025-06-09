using Blackbird.Applications.Sdk.Common;

namespace Apps.Gmail.Models.Responses;

public class SearchEmailsResponse
{
    [Display("Email IDs")]
    public IEnumerable<string> EmailIds { get; set; }
}
