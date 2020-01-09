using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapperEngine.Infrastructure.Models
{
    public class WebScrapperForms
    {
        public string GetURL { get; set; }

        public string PostURL { get; set; }

        public string RequestType { get; set; }

        public string Name { get; set; }

        public List<WebScrapperFormInputs> WebScrapperFormInputs { get; set; }

        public List<RegexFormInput> RegexFormInputs { get; set; }


        public WebScrapperForms()
        {
            WebScrapperFormInputs = new List<WebScrapperFormInputs>();
            RegexFormInputs = new List<RegexFormInput>();
        }
    }
}
