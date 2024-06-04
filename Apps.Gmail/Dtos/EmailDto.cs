using Google.Apis.Gmail.v1.Data;

namespace Apps.Gmail.Dtos
{
    public class EmailDto
    {
        public EmailDto(Message message)
        {
            Subject = message.Payload.Headers.First(x => x.Name == "Subject").Value;
        }

        public string Subject { get; set; }
    }
}
