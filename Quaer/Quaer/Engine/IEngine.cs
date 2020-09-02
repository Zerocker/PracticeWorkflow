using OpenQA.Selenium;
using System.Collections.Generic;
using Quaer.Model;

namespace Quaer.Engine
{
    public interface IEngine
    {
        string Name { get; set; }

        IWebDriver Driver { get; set; }

        List<Result> Results { get; set; }

        void SetQuery(string query);

        void DoSearch();
    }
}