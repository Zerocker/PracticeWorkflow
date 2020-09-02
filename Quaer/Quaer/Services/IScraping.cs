using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quaer.Engine;
using Quaer.Model;
using Quaer.Utils;

namespace Quaer.Services
{
    public interface IScraping
    {
        ObservableCollection<Selectable<string>> Queries { get; set; }

        List<IEngine> AvailableEngines { get; }

        List<IEngine> Engines { get; set; }

        IEnumerable<string> LoadQueries(string path = null);

        IEnumerable<Result> ProcessSearch(string query);
    }
}
