using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Quaer.Engine;
using Quaer.Model;
using Quaer.Utils;

namespace Quaer.Services
{
    /// <summary>
    /// Service for handling search operations
    /// </summary>
    public class ScrapingService : IScraping
    {
        /// <summary>
        /// The list of displayed search queries
        /// </summary>
        public ObservableCollection<Selectable<string>> Queries { get; set; }

        /// <summary>
        /// The list of all search engine instances
        /// </summary>
        public List<IEngine> AvailableEngines { get; private set; }

        /// <summary>
        /// The list of selected search engine instances
        /// </summary>
        public List<IEngine> Engines { get; set; }

        /// <summary>
        /// Reference to the dialog service
        /// </summary>
        private IDialogService dialog;

        /// <summary>
        /// Reference to the file service
        /// </summary>
        private IFileService<string> file;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialog">Reference to the dialog service</param>
        /// <param name="file">Reference to the file service</param>
        public ScrapingService(IDialogService dialog, IFileService<string> file)
        {
            this.dialog = dialog;
            this.file = file;

            AvailableEngines = new List<IEngine>()
            {
                new Google(null), new Yandex(null), new MailRu(null),
                new Bing(null), new DuckDuckGo(null)
            };

            ChromeHelper.Initialize();
        }

        /// <summary>
        /// Loads list of queries from the text file
        /// </summary>
        /// <param name="path">Path to the text file</param>
        /// <returns>The list of queries</returns>
        public IEnumerable<string> LoadQueries(string path)
        {
            if (path == null)
                if (dialog.OpenFileDialog() == true)
                    path = dialog.FilePath;

            var lines = file.Open(path);
            if (lines.Count() <= 0)
                throw new Exception("File is empty!");
            else
                return lines;
        }

        /// <summary>
        /// Searches the query string in selected search engines
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The list of search results</returns>
        public IEnumerable<Result> ProcessSearch(string query)
        {
            List<Result> results = new List<Result>();

            foreach (var e in Engines)
            {
                e.Driver = ChromeHelper.Driver;
                e.SetQuery(query);
                e.DoSearch();
                results.AddRange(e.Results);
            }
            
            return results;
        }
    }
}
