using Apps.Gmail.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Gmail.Models.Responses
{
    public class SearchEmailsResponse
    {
        public List<EmailDto> Emails { get; set; }
    }
}
