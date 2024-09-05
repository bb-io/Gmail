using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Gmail.Models.Requests
{
    public class SendEmailRequest
    {
        public string To { get; set; }

        public string? Subject { get; set; }

        public string? Message { get; set; }

        public List<string>? CC { get; set; }

        public List<FileReference>? Attachments { get; set; }
    }
}
