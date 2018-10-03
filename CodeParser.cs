using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace TestPars
{
    public class CodeParser
    {
        public string urlLinks = @"c:\d\links.html";

        private HtmlDocument GetDocument(string url)
        {
            HtmlDocument doc;
            var _request = (FileWebRequest)WebRequest.Create(url);
            var _response = (FileWebResponse)_request.GetResponse();

            using (var reader = new StreamReader(_response.GetResponseStream(), Encoding.GetEncoding(65001)))
            {
                string html = reader.ReadToEnd();
                doc = new HtmlDocument();
                doc.LoadHtml(html);
            }
            _response.Close();
            return doc;
        }

        private void FillLinks(string[] matches, HtmlNodeCollection runNodes, List<string> links)
        {
            foreach (var node in runNodes)
            {
                if (node.Attributes.Contains("href"))
                {
                    if (matches.All(p => node.Attributes["href"].Value.Contains(p)))
                        links.Add(node.Attributes["href"].Value);
                }
                FillLinks(matches, node.ChildNodes, links);
            }
        }
        private void FillCodes(HtmlDocument document, List<DepartmentInfo> codes)
        {
            var _tdNodes = document.DocumentNode.SelectNodes("//td[@class='line-content']").Where(p => p.InnerText.Contains("photo_city")).ToArray();

            for (int i = 0; i < _tdNodes.Length; i += 2)
            {
                var _name = _tdNodes[i].ChildNodes[1].InnerText.TrimStart(' ');
                var _code = _tdNodes[i + 1].ChildNodes[1].InnerText;
                codes.Add(new DepartmentInfo() { Name = _name, Code = _code });
            }
        }
        public List<DepartmentInfo> GetDepartmentCodes()
        {
            var _document = GetDocument(urlLinks);
            var _matches = new string[] { @"/spravochnik-kodov-ufms-rossii/", "kody" };
            var _links = new List<string>(53);

            FillLinks(_matches, _document.DocumentNode.ChildNodes, _links);

            var _codes = new List<DepartmentInfo>();
            foreach (var _url in _links)
            {
                _document = GetDocument(@"c:\d\codes.html");
                FillCodes(_document, _codes);
            }
            return _codes;
        }
        
    }
}
