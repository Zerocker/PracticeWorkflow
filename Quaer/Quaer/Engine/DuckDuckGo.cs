using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using HtmlAgilityPack;
using Quaer.Model;
using OpenQA.Selenium.Support.UI;

namespace Quaer.Engine
{
    /// <summary>
    /// Class for making search queries in DuckDuckGo
    /// </summary>
    public class DuckDuckGo : BaseEngine
    {
        /// <summary>
        /// Template URL to create a search query
        /// </summary>
        private readonly string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public DuckDuckGo(IWebDriver driver) : base(driver)
        {
            Url = "https://html.duckduckgo.com/html/?q={0}";
        }

        /// <summary>
        /// Proceeds search in DuckDuckGo (HTML version)
        /// </summary>
        public override void DoSearch()
        {
            HtmlDocument html = new HtmlDocument();

            if (QueryRegex == null)
                throw new Exception("Query is not set!");

            this.Driver.Url = this.queryType switch
            {
                QueryType.Number => string.Format(Url, QueryNumber.ToString()),
                QueryType.String => string.Format(Url, Regex.Replace(QueryString, @"\s+", "+")),
                _ => string.Format(Url, "Google"),
            };
            
            while (true)
            {
                html.LoadHtml(this.Driver.PageSource);
                HtmlNodeCollection results = html.DocumentNode.SelectNodes(".//div[@class='links_main links_deep result__body']");

                if (results == null)
                    break;

                for (int i = 0; i < results.Count; i++)
                {
                    var anchor = results[i].SelectSingleNode(".//a[@class='result__a']");
                    var snippet = results[i].SelectSingleNode(".//a[@class='result__snippet']");

                    if (anchor != null && snippet != null)
                    {
                        string link = anchor.Attributes["href"].Value;
                        string title = anchor.InnerText;
                        string description = snippet.InnerText;

                        if (this.FindDatabase(title, description, false))
                            this.Results.Add(new Result(link, title, description, QueryNumber.ToString()));
                    }
                }

                // Check presence of 'Next' button...
                var by = By.XPath("//*[@class='btn btn--alt' and @value='Next']");
                if (Driver.FindElements(by).Count == 0)
                    break;
                
                // ... and click on it!
                var next = Driver.FindElement(by);
                if (next != null)
                    next.Click();
                else
                    break;
            }
        }
    }
}
