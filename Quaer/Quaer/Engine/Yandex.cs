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
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Quaer.Engine
{
    /// <summary>
    /// Class for making search queries in Yandex
    /// </summary>
    public class Yandex : BaseEngine
    {
        /// <summary>
        /// Template URL to create a search query
        /// </summary>
        private readonly string Url;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public Yandex(IWebDriver driver) : base(driver)
        {
            Url = "https://yandex.ru/yandsearch?text={0}&p={1}&lr=213";
        }

        /// <summary>
        /// Proceeds search in Yandex
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
                if (!Driver.WaitUntilPresent(By.CssSelector(".main.serp.i-bem.main_js_inited.serp_js_inited"), 3))
                    break;

                html.LoadHtml(this.Driver.PageSource);

                var serp = html.DocumentNode.SelectNodes("//li[@class='serp-item']");

                // Get current page index
                var onCurrentPage = html.DocumentNode.SelectSingleNode("//span[@class='pager__item pager__item_current_yes pager__item_kind_page']");

                if (onCurrentPage == null || serp == null)
                    break;
                if (int.Parse(onCurrentPage.InnerText) != page+1)
                    break;
                else
                    page++;

                for (int i = 0; i < serp.Count; i++)
                {
                    if (serp[i].Attributes["data-cid"] == null)
                        continue;
                    
                    var link = serp[i].SelectSingleNode(".//a").Attributes["href"].Value;
                    var urlText = serp[i].SelectSingleNode(".//div[@class='organic__url-text']");
                    var fullText = serp[i].SelectSingleNode(".//span[@class='extended-text__full']");

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
