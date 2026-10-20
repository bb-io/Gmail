using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Google.Apis.Gmail.v1.Data;
using System.Text;

namespace Apps.Gmail.Dtos;

public class EmailDto
{
    public EmailDto(Message message)
    {
        Id = message.Id;
        Subject = message.Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value ?? "";
        Snippet = message.Snippet;
        var fromFull = message.Payload.Headers.FirstOrDefault(x => x.Name == "From")?.Value ?? "";
        From = (fromFull != null && fromFull.Contains(" <")) ? fromFull.Split(" <")[0] : fromFull;
        FromEmail = (fromFull != null && fromFull.Contains(" <")) ? fromFull.Split(" <")[1].TrimEnd('>') : fromFull;
        var toFull = message.Payload.Headers.FirstOrDefault(x => x.Name == "To")?.Value ?? "";
        To = (toFull != null && toFull.Contains(" <")) ? toFull.Split(" <")[0] : toFull;
        ToEmail = (toFull != null && toFull.Contains(" <")) ? toFull.Split(" <")[1].TrimEnd('>') : toFull;

        Message = GetMessageBody(message.Payload);
    }

    [Display("Email ID")]
    public string Id { get; set; }

    public string Subject { get; set; }

    public string Snippet { get; set; }

    [Display("Sender name")]
    public string? From { get; set; }

    [Display("Sender email")]
    public string? FromEmail { get; set; }

    [Display("Receiver name")]
    public string? To { get; set; }

    [Display("Receiver email")]
    public string? ToEmail { get; set; }

    public string Message { get; set; }

    public IEnumerable<FileReference> Attachments { get; set; } = Enumerable.Empty<FileReference>();

    private static string GetMessageBody(MessagePart payload)
    {
        var bodyParts = Traverse(payload)
            .Where(x => !IsAttachment(x))
            .ToList();

        var bodyPart = bodyParts.FirstOrDefault(x =>
                           string.Equals(x.MimeType, "text/plain", StringComparison.OrdinalIgnoreCase) &&
                           !string.IsNullOrEmpty(x.Body?.Data))
                       ?? bodyParts.FirstOrDefault(x =>
                           string.Equals(x.MimeType, "text/html", StringComparison.OrdinalIgnoreCase) &&
                           !string.IsNullOrEmpty(x.Body?.Data));

        // Preserve support for a non-multipart payload even if Gmail omitted its MIME type.
        var bodyBase64 = bodyPart?.Body?.Data ?? payload.Body?.Data;
        return string.IsNullOrEmpty(bodyBase64) ? string.Empty : DecodeBase64Url(bodyBase64);
    }

    private static IEnumerable<MessagePart> Traverse(MessagePart part)
    {
        yield return part;

        if (part.Parts == null)
        {
            yield break;
        }

        foreach (var child in part.Parts)
        {
            foreach (var descendant in Traverse(child))
            {
                yield return descendant;
            }
        }
    }

    private static bool IsAttachment(MessagePart part)
    {
        if (!string.IsNullOrEmpty(part.Filename))
        {
            return true;
        }

        var contentDisposition = part.Headers?
            .FirstOrDefault(x => string.Equals(x.Name, "Content-Disposition", StringComparison.OrdinalIgnoreCase))?
            .Value;

        return contentDisposition?.StartsWith("attachment", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string DecodeBase64Url(string value)
    {
        var base64 = value.Replace("-", "+").Replace("_", "/");
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');

        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}
