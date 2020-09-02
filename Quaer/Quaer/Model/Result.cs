using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Quaer.Utils;

namespace Quaer.Model
{
    /// <summary>
    /// Class for the search result
    /// </summary>
    public class Result : Observable, IDisposable
    {
        private string _link;
        private string _title;
        private string _description;
        private string _query;

        /// <summary>
        /// List of created instances of the class
        /// </summary>
        private static readonly List<bool> Counter = new List<bool>();
        
        /// <summary>
        /// The lock object
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The number of created instance of the class
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The link of the web page
        /// </summary>
        public string Link
        {
            get => _link;
            set => Set(ref _link, value);
        }

        /// <summary>
        /// The title of the web page
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        /// <summary>
        /// The description of the web page
        /// </summary>
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        /// <summary>
        /// The search query
        /// </summary>
        public string Query
        {
            get => _query;
            set => Set(ref _query, value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="link">The link of the web page</param>
        /// <param name="title">The title of the web page</param>
        /// <param name="description">The description of the web page</param>
        public Result(string link, string title, string description, string query)
        {
            Link = link;
            Title = title;
            Description = description;
            Query = query;

            lock (Lock)
            {
                int next = GetAvailableIndex();
                if (next < 0)
                {
                    next = Counter.Count;
                    Counter.Add(true);
                }
                Id = next;
            } 
        }

        /// <summary>
        /// Gets available index for creating new instance of the class
        /// </summary>
        /// <returns>The index of created instance of the class</returns>
        private int GetAvailableIndex()
        {
            // The instance count starts at 1
            for (int i = 1; i <= Counter.Count; i++)
            {
                if (Counter[i-1] == false)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Removes current disposed instance from the Counter list
        /// </summary>
        public void Dispose()
        {
            lock (Lock)
            {
                Counter[Id] = false;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        public override string ToString()
        {
            return $"{Query}\n{Title}\n{Link}\n\n";
        }
    }
}
