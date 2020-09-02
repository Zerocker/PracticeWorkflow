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
using Quaer.Utils;

namespace Quaer.Engine
{
    /// <summary>
    /// Class for making search queries in Bing
    /// </summary>
    public class Bing : BaseEngine
    {
        /// <summary>
        /// Template URL to create a search query
        /// </summary>
        private readonly string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public Bing(IWebDriver driver) : base(driver)
        {
            Url = "https://www.bing.com/search?q={0}&first={1}";
        }

        /// <summary>
        /// Proceeds search in Bing
        /// </summary>
        public override void DoSearch()
        {
            int page = 1;
            int total = 0;
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

                // Wait until page is fully loaded
                if (!Driver.WaitUntilPresent(By.Id("b_tween"), 3))
                    break;

                html.LoadHtml(this.Driver.PageSource);
                HtmlNodeCollection algo = html.DocumentNode.SelectNodes(".//li[@class='b_algo']");

                // Gets total count of results for 'first' value in url
                if (total == 0)
                {
                    HtmlNode count = html.DocumentNode.SelectSingleNode(".//span[@class='sb_count']");
                    int[] digits = Number.GetDigits(count.InnerText).ToArray();
                    total = digits[digits.Length - 1];
                }

                if (algo == null || page > total)
                    break;
                else
                    page += algo.Count+1;

                for (int i = 0; i < algo.Count; i++)
                {
                    var link = algo[i].SelectSingleNode(".//a").Attributes["href"].Value;
                    var urlText = algo[i].SelectSingleNode(".//a");
                    var fullText = algo[i].SelectSingleNode(".//p");

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
