using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebScrapperEngine.Infrastructure.Helpers;
namespace WebScrapperEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var filepath = @"../../TestingXMLFile/ScrapperXMLFinal.xml";
            XElement rootElement = XElement.Load(filepath);
            WebScrapperHelper WebScrapperHelper = new WebScrapperHelper();
            var response= WebScrapperHelper.ExecuteWebScrapper(rootElement);

        }
    }
}
