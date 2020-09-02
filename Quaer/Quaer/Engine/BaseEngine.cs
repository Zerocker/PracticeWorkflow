using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Quaer.Model;

namespace Quaer.Engine
{
    public enum QueryType
    {
        None,
        String,
        Number,
    }

    /// <summary>
    /// Basic template for the search engine parsing
    /// </summary>
    public abstract class BaseEngine : IEngine
    {
        /// <summary>
        /// The reference to Selenium web driver instance
        /// </summary>
        public IWebDriver Driver { get; set; }

        /// <summary>
        /// List of search results
        /// </summary>
        public List<Result> Results { get; set; }

        /// <summary>
        /// Name of search enghine
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Processed query as an object of class Number
        /// </summary>
        protected Number QueryNumber;

        /// <summary>
        /// Unprocessed query as a string
        /// </summary>
        protected string QueryString;

        /// <summary>
        /// List of words to detect the database of phone numbers
        /// </summary>
        protected string[] Blocklist;

        /// <summary>
        /// Regex for detecting all phone numbers
        /// </summary>
        private readonly Regex AllPhones;

        /// <summary>
        /// Regex for detecting specific phone number
        /// </summary>
        protected Regex QueryRegex;

        /// <summary>
        /// Type of search query
        /// </summary>
        protected QueryType queryType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        public BaseEngine(IWebDriver driver)
        {
            Driver = driver;
            AllPhones = new Regex("((8|\\+7)[\\- ]?)?[\\s.\\(\\-]?[0-9]{3}[\\s.\\)\\-]?[0-9\\s]{2,3}[\\s\\-]?[0-9]{2,3}[0-9\\-]{2,}");
            Blocklist = new string[] { "чей номер", "звонил", "спам", "регион", "список", "информация",
                                       "оператор", "нумерологический", "отзывы", "телефонов", "код" };
            Results = new List<Result>();
            Name = this.GetType().Name;
        }

        /// <summary>
        /// Sets query for search
        /// </summary>
        /// <param name="query">Query string</param>
        public void SetQuery(string query)
        {
            if (Number.IsNumber(query))
            {
                QueryNumber = new Number(query);
                QueryRegex = new Regex(QueryNumber.GenerateRegex());
                queryType = QueryType.Number;
            }
            else
            {
                QueryString = query;
                QueryRegex = new Regex($"^.*\b({query})\b.*$ ");
                queryType = QueryType.String;
            }
        }

        /// <summary>
        /// Checks for a database of phone numbers
        /// </summary>
        /// <param name="title">The title of web page</param>
        /// <param name="text">The description of web page</param>
        /// <param name="removeSpaces">Removes spaces in both param strings</param>
        /// <returns></returns>
        protected bool FindDatabase(string title, string text, bool removeSpaces = true)
        {
            title = title.ToLower();
            text = text.ToLower();

            if (Blocklist.Any(b => title.Contains(b)) || Blocklist.Any(b => text.Contains(b)))
                return false;

            int total = 0, count = 0;
            if (removeSpaces)
                text = Regex.Replace(text, @"\s+", string.Empty);
            MatchCollection matches = AllPhones.Matches(text);
            
            foreach(Match match in matches)
            {
                if (QueryRegex.IsMatch(match.Value))
                    count++;
                total++;
            }

            if (count != total)
                return false;
            return true;
        }

        public abstract void DoSearch();
    }
}
