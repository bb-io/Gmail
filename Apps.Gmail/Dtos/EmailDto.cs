using Blackbird.Applications.Sdk.Common;
using Google.Apis.Gmail.v1.Data;

namespace Apps.Gmail.Dtos
{
    public class EmailDto
    {
        public EmailDto(Message message)
        {
            Id = message.Id;
            Subject = message.Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value ?? "";

            var fromFull = message.Payload.Headers.FirstOrDefault(x => x.Name == "From")?.Value;
            var from = (fromFull != null && fromFull.Contains(" <")) ? fromFull.Split(" <")[0] : fromFull;
            DisplayName = $"{from} {Subject}";
        }

        public string Id { get; set; }

        public string Subject { get; set; }

        [Display("Display name")]
        public string DisplayName { get; set; }
    }
}
