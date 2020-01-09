using HtmlAgilityPack;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebScrapperEngine.Infrastructure.Constants;
using WebScrapperEngine.Infrastructure.Models;

namespace WebScrapperEngine.Infrastructure.Helpers
{
    public class WebScrapperHelper
    {

        public static string sessionID="";
        public static string sessionIDToken = "";
        public static string RequestVerificationToken = "";


        public WebScrapperResponse ExecuteWebScrapper(XElement root)
        {
            var WebScrapperFormList = GenerateWebForms(root);
            var response = ExecuteWebScrapper(WebScrapperFormList);

            return response;
        }

        public List<WebScrapperForms> GenerateWebForms(XElement root)
        {
            List<WebScrapperForms> WebScrapperFormsList = new List<WebScrapperForms>();
            var formNodes = root.Descendants(XMLConstants.FORM);
            var sessionNodes = root.Descendants(XMLConstants.SESSION).Descendants(XMLConstants.INPUT);

            sessionIDToken = sessionNodes.Where(p => p.Attribute(XMLConstants.NAME).Value == XMLConstants.SESSION_ID_TOKEN).FirstOrDefault().Value;
            RequestVerificationToken = sessionNodes.Where(p => p.Attribute(XMLConstants.NAME).Value == XMLConstants.REQUEST_VERIFICATION_TOKEN).FirstOrDefault().Value;

            foreach (var formNode in formNodes)
            {               
                WebScrapperFormsList.Add(GetWebFormInputFromXMLNode(formNode));

            }

            return WebScrapperFormsList;
        }

        public WebScrapperResponse ExecuteWebScrapper(List<WebScrapperForms> WebScrapperFormList)
        {
            WebScrapperResponse webScrapperResponse = new WebScrapperResponse();
            List<WebScrapperFormResponse> FormResponseList = new List<WebScrapperFormResponse>();
            foreach (WebScrapperForms WebScrapperForm in WebScrapperFormList)
            {
                WebScrapperFormResponse response = new WebScrapperFormResponse();
                response = HitWebPage(WebScrapperForm);
                FormResponseList.Add(response);

            }
            webScrapperResponse.WebScrapperFormResponse = FormResponseList;


            return webScrapperResponse;
        }

        public WebScrapperFormResponse HitWebPage(WebScrapperForms form)
        {
            WebScrapperFormResponse webscrapperformResponse = new WebScrapperFormResponse();
            var client_GET = new RestClient(form.GetURL);            
            if (form.RequestType == ActionConstants.GET)
            {                               
                var GetRequest = new RestRequest("", Method.GET);
                if (sessionID != "")
                {
                    GetRequest.AddCookie(sessionIDToken, sessionID);
                }

                foreach (var param in form.WebScrapperFormInputs)
                {
                    GetRequest.AddParameter(param.Name, param.Value);
                }

                var responseGetRequest = client_GET.Execute(GetRequest);
                var regexResponseList = RegexHelper.GetRegexResponse(responseGetRequest.Content,form.RegexFormInputs);
                webscrapperformResponse.regexResponseList = regexResponseList;
                webscrapperformResponse.FormResponse = responseGetRequest.Content;
                webscrapperformResponse.FormAction = form.GetURL;
                return webscrapperformResponse;
            }
            else if (form.RequestType == ActionConstants.POST)
            {
                var client_POST = new RestClient(form.PostURL);
                var GetRequest = new RestRequest("", Method.GET);
                if (sessionID != "")
                {
                    GetRequest.AddCookie(sessionIDToken, sessionID);
                }
                var responseGetRequest = client_GET.Execute(GetRequest);
                var cookies = responseGetRequest.Cookies.ToList();

                var html = responseGetRequest.Content;
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var tokenNode = doc.DocumentNode.SelectNodes(string.Format("//input[contains(@{0},'{1}')]", XMLConstants.NAME, RequestVerificationToken)).FirstOrDefault();
                var tokenvalue = tokenNode.Attributes.Where(p => p.Name == "value").FirstOrDefault().Value; 

              
                var PostRequest = new RestRequest("", Method.POST);
                foreach (var cookie in cookies)
                {
                    PostRequest.AddCookie(cookie.Name,cookie.Value);
                    if (cookie.Name == sessionIDToken)
                    {
                        sessionID = cookie.Value;
                    }
                }
                PostRequest.AddParameter(RequestVerificationToken, tokenvalue);
                if (sessionID !="")
                {
                    PostRequest.AddCookie(sessionIDToken, sessionID);
                }
                foreach (var param in form.WebScrapperFormInputs)
                {
                    PostRequest.AddParameter(param.Name,param.Value);
                }

                var responsePostequest = client_POST.Execute(PostRequest);
                var regexResponseList = RegexHelper.GetRegexResponse(responsePostequest.Content, form.RegexFormInputs);
                webscrapperformResponse.regexResponseList = regexResponseList;
                webscrapperformResponse.FormResponse = responsePostequest.Content;
                webscrapperformResponse.FormAction = form.PostURL;
                return webscrapperformResponse;
            }
        
            
            return null;
        }

        public WebScrapperForms GetWebFormInputFromXMLNode(XElement node)
        {
            WebScrapperForms WebScrapperForm = new WebScrapperForms();
            WebScrapperForm.Name = node.Attribute(XMLConstants.NAME).Value;
            WebScrapperForm.RequestType = node.Attribute(XMLConstants.TYPE).Value;
            WebScrapperForm.GetURL = node.Attribute(XMLConstants.GET_URL).Value;
            WebScrapperForm.PostURL = node.Attribute(XMLConstants.POST_URL).Value;
            List<WebScrapperFormInputs> FormInputList = new List<WebScrapperFormInputs>();
            List<RegexFormInput> RegexFormInputList = new List<RegexFormInput>();
            var ParametersNodeList = node.Descendants(XMLConstants.INPUT);
            var RegexNodeList = node.Descendants(XMLConstants.REGEX).Descendants(XMLConstants.REGEX_INPUT);
            foreach (var parameterNode in ParametersNodeList)
            {
                WebScrapperFormInputs webscrapperforminput = new WebScrapperFormInputs();
                webscrapperforminput.Id = parameterNode.Attribute(XMLConstants.ID).Value;
                webscrapperforminput.Name = parameterNode.Attribute(XMLConstants.NAME).Value;
                webscrapperforminput.Value = parameterNode.Value;
                FormInputList.Add(webscrapperforminput);

            }
            foreach (var regexNode in RegexNodeList)
            {
                RegexFormInput regexFormInput = new RegexFormInput();
                regexFormInput.Name = regexNode.Attribute(XMLConstants.NAME).Value;
                regexFormInput.Tag = regexNode.Attribute(XMLConstants.TAG).Value;
                regexFormInput.Attribute = regexNode.Attribute(XMLConstants.ATTRIBUTE).Value;
                regexFormInput.AttributeValue = regexNode.Value;
                RegexFormInputList.Add(regexFormInput);

            }
            WebScrapperForm.WebScrapperFormInputs = FormInputList;
            WebScrapperForm.RegexFormInputs = RegexFormInputList;

            return WebScrapperForm;
        }
    }
}

