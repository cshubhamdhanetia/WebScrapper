using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScrapperEngine.Infrastructure.Models;

namespace WebScrapperEngine.Infrastructure.Helpers
{
    public static class RegexHelper
    {
        public static List<RegexFormResponse> GetRegexResponse(string Content, List<RegexFormInput> regexFormInputList)
        {
            List<RegexFormResponse> RegexFormResponseList = new List<RegexFormResponse>();

            foreach (var regexInput in regexFormInputList)
            {
                RegexFormResponseList.Add(ExecuteRegexOperation(Content,regexInput));
            }

            return RegexFormResponseList;
        }

        private static RegexFormResponse ExecuteRegexOperation(string Content, RegexFormInput regexInput)
        {
            RegexFormResponse regexFormResponse = new RegexFormResponse();
            regexFormResponse.name = regexInput.Name;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Content);
            regexFormResponse.response = doc.DocumentNode.SelectSingleNode(string.Format("//{0}[contains(@{1},'{2}')]", regexInput.Tag, regexInput.Attribute, regexInput.AttributeValue)).InnerHtml;
            
            return regexFormResponse;
        }

    }
}
