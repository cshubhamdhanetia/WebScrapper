using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapperEngine.Infrastructure.Models
{
    public class WebScrapperFormResponse
    {
        public string FormAction { get; set; }
        public string FormResponse { get; set; }
        public List<RegexFormResponse> regexResponseList { get; set; }

    }
}
