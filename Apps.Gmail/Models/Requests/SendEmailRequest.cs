using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Gmail.Models.Requests;

public class SendEmailRequest
{
    [Display("Receiver name")]
    public string To { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public List<string>? CC { get; set; }

    public List<FileReference>? Attachments { get; set; }
}
