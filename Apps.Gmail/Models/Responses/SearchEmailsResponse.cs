using Apps.Gmail.Dtos;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Gmail.Models.Responses
{
    public class SearchEmailsResponse
    {
        [Display("Email IDs")]
        public IEnumerable<string> EmailIds { get; set; }
    }
}
