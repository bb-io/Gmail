using Google.Apis.Gmail.v1.Data;

namespace Apps.Gmail.Dtos
{
    public class EmailDto
    {
        public EmailDto(Message message)
        {
            Id = message.Id;
            Subject = message.Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value ?? "";
        }

        public string Id { get; set; }

        public string Subject { get; set; }
    }
}
