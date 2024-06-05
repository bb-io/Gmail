using Blackbird.Applications.Sdk.Common;

namespace Apps.Gmail.Models.Requests
{
    public class SearchEmailsRequest
    {
        [Display("Query", Description = "Specify query the same way as in gmail search box")]
        public string? Query { get; set; }
    }
}
