using Google.Apis.Gmail.v1.Data;
using System.ComponentModel.DataAnnotations;

namespace Apps.Gmail.Dtos
{
    public class EmailDto
    {
        public EmailDto(Message message)
        {
            Id = message.Id;
            Subject = message.Payload.Headers.FirstOrDefault(x => x.Name == "Subject")?.Value ?? "";

            var from = message.Payload.Headers.FirstOrDefault(x => x.Name == "From")?.Value?.Split(" <")[0] ?? "";
            Name = $"{from} {Subject}";
        }

        public string Id { get; set; }

        public string Subject { get; set; }

        public string Name { get; set; }
    }
}
