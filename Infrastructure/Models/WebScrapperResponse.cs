using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapperEngine.Infrastructure.Models
{
    public class WebScrapperResponse
    {
        public string ErrorDetails { get; set; }

        public string status { get; set; }
        public List<WebScrapperFormResponse> WebScrapperFormResponse {get;set;}

        public WebScrapperResponse()
        {
            WebScrapperFormResponse = new List<Models.WebScrapperFormResponse>();
        }
    }
}
