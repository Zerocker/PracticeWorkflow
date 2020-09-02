using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Quaer.Engine;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace Testing
{
    [TestClass]
    public class EngineTest
    {
        private ChromeDriver Driver = null;

        private readonly string ProfileName = "rcoiutut.braxe";

        private readonly string TestQuery = "+7-914-111-1111";

        public void Init()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var options = new ChromeOptions();
            options.AddArguments("user-data-dir=" + ProfileName);
            Driver = new ChromeDriver(path, options);
        }
        
        [TestMethod]
        public void TestGoogle()
        {
            Init();

            Google google = new Google(Driver);
            google.SetQuery(TestQuery);
            google.DoSearch();
            google.Driver.Close();

            var unique = google.Results.Distinct();
            Assert.IsTrue(unique.Count() > 10);
        }

        [TestMethod]
        public void TestYandex()
        {
            Init();

            Yandex yandex = new Yandex(Driver);
            yandex.SetQuery(TestQuery);
            yandex.DoSearch();
            yandex.Driver.Close();

            var unique = yandex.Results.Distinct();
            Assert.IsTrue(unique.Count() > 10);
        }

        [TestMethod]
        public void TestMailRu()
        {
            Init();

            MailRu mailru = new MailRu(Driver);
            mailru.SetQuery(TestQuery);
            mailru.DoSearch();
            mailru.Driver.Close();

            var unique = mailru.Results.Distinct();
            Assert.IsTrue(unique.Count() > 10);
        }

        [TestMethod]
        public void TestBing()
        {
            Init();

            Bing bing = new Bing(Driver);
            bing.SetQuery(TestQuery);
            bing.DoSearch();
            bing.Driver.Close();

            var unique = bing.Results.Distinct();
            Assert.IsTrue(unique.Count() > 5);
        }

        [TestMethod]
        public void TestDuckDuckGo()
        {
            Init();

            DuckDuckGo ddg = new DuckDuckGo(Driver);
            ddg.SetQuery(TestQuery);
            ddg.DoSearch();
            ddg.Driver.Close();

            var unique = ddg.Results.Distinct();
            Assert.IsTrue(unique.Count() > 10);
        }
    }
}
