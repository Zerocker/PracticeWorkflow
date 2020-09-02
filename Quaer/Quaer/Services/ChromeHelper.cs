using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Quaer.Utils;

namespace Quaer.Services
{
    /// <summary>
    /// Class helper for initializing Chrome web browser and driver
    /// </summary>
    public static class ChromeHelper
    {
        /// <summary>
        /// The name of used profile
        /// </summary>
        public static string Profile = "rcoiutut.braxe";

        /// <summary>
        /// The executable absolute path
        /// </summary>
        public static string AssemblyPath;

        /// <summary>
        /// Selenium web driver instance
        /// </summary>
        public static IWebDriver Driver;

        /// <summary>
        /// Detects the current version of installed Chrome
        /// </summary>
        /// <returns>Installed version of Chrome</returns>
        public static string DetectVersion()
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path != null)
                return Extensions.GetStringKey("ChromeHelper_Version") + FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            return null;
        }

        /// <summary>
        /// Checks the presence of Chrome user profile
        /// </summary>
        /// <param name="dialog">Reference to dialog service</param>
        public static void CheckProfile(IDialogService dialog)
        {
            AssemblyPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            string profile = Path.Combine(AssemblyPath, Profile);
            
            if (!Directory.Exists(profile))
            {
                dialog.ShowInfo(Extensions.GetStringKey("ChromeHelper_Check"));
                OpenWithProfile();
            }
        }

        /// <summary>
        /// Opens Chrome window via OS
        /// </summary>
        public static void OpenWithProfile()
        {
            string chrome = Environment.GetEnvironmentVariable("ProgramW6432") + @"\Google\Chrome\Application\chrome.exe";
            string profile = Path.Combine(AssemblyPath, Profile);

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = chrome,
                    Arguments = $"--user-data-dir={profile} --no-first-run --no-default-browser-check",
                }
            };
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Opens Chrome window via Selenium web driver
        /// </summary>
        public static void Initialize()
        {
            var options = new ChromeOptions();
            options.AddArguments("user-data-dir=" + Path.Combine(AssemblyPath, Profile));

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            Driver = new ChromeDriver(service, options);
            Driver.Manage().Window.Minimize();
        }
    }
}
