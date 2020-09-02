using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using HtmlAgilityPack;
using Quaer.Model;

namespace Quaer.Engine
{
    /// <summary>
    /// Class for making search queries in Mail.Ru
    /// </summary>
    public class MailRu : BaseEngine
    {
        /// <summary>
        /// Template URL to create a search query
        /// </summary>
        private readonly string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public MailRu(IWebDriver driver) : base(driver)
        {
            Url = "https://go.mail.ru/search?fm=1&q={0}&sf={1}";
        }

        /// <summary>
        /// Proceeds search in Mail.Ru
        /// </summary>
        public override void DoSearch()
        {
            int page = 0;
            HtmlDocument html = new HtmlDocument();

            if (QueryRegex == null)
                throw new Exception("Query is not set!");

            while (true)
            {
                this.Driver.Url = this.queryType switch
                {
                    QueryType.Number => string.Format(Url, QueryNumber.ToString(), page),
                    QueryType.String => string.Format(Url, Regex.Replace(QueryString, @"\s+", "+"), page),
                    _ => string.Format(Url, "Google", page),
                };
                
                this.Driver.Navigate();
                html.LoadHtml(this.Driver.PageSource);

                var li = html.DocumentNode.SelectNodes("//li[@class='result__li']");
                if (li == null)
                    break;
                else
                    page += li.Count;

                for (int i = 0; i < li.Count; i++)
                {
                    var link = li[i].SelectSingleNode(".//a").Attributes["href"].Value;
                    var urlText = li[i].SelectSingleNode(".//h3[@class='result__title']");
                    var fullText = li[i].SelectSingleNode(".//div[@class='SnippetResult-result']");

                    if (urlText != null && fullText != null)
                    {
                        string title = urlText.InnerText;
                        string description = fullText.InnerText;

                        if (this.FindDatabase(title, description))
                            this.Results.Add(new Result(link, title, description, QueryNumber.ToString()));
                    }
                }
            }
        }
    }
}
