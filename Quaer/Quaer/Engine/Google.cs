using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using HtmlAgilityPack;
using Quaer.Model;
using Quaer.Utils;

namespace Quaer.Engine
{
    /// <summary>
    /// Class for making search queries in Google
    /// </summary>
    public class Google : BaseEngine
    {
        /// <summary>
        /// Template URL to create a search query
        /// </summary>
        private readonly string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public Google(IWebDriver driver) : base(driver)
        {
            Url = "https://google.com/search?q={0}&start={1}";
        }

        /// <summary>
        /// Proceeds search in Google
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

                // Wait until page is fully loaded
                if (!Driver.WaitUntilPresent(By.CssSelector(".gb_ta.gb_he.gb_5a.gb_Pc"), 3))
                    break;

                this.Driver.Navigate();
                html.LoadHtml(this.Driver.PageSource);

                var r = html.DocumentNode.SelectNodes("//div[@class='r']");
                var st = html.DocumentNode.SelectNodes("//span[@class='st']");

                if (r == null || r.Count != st.Count)
                    break;

                for (int i = 0; i < r.Count; i++)
                {
                    var link = r[i].SelectSingleNode(".//a").Attributes["href"].Value;
                    var title = r[i].SelectSingleNode(".//h3").InnerText;
                    var description = st[i].InnerText;

                    if (this.FindDatabase(title, description))
                        this.Results.Add(new Result(link, title, description, QueryNumber.ToString()));
                }

                page += r.Count;
            }
        }
    }
}
