using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Google.Apis.Gmail.v1.Data;
using System.Text;

namespace Apps.Gmail.Dtos
{
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

            var messageBase64 = message.Payload.Parts.FirstOrDefault(x => x.MimeType == "text/plain")?.Body.Data ?? "";
            var base64EncodedBytes = Convert.FromBase64String(messageBase64.Replace("-", "+").Replace("_", "/"));
            Message = Encoding.UTF8.GetString(base64EncodedBytes);
        }

        [Display("ID")]
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

        public IEnumerable<FileReference> Attachments { get; set; }
    }
}
