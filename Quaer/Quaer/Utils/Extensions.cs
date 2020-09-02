using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Quaer.Model;

namespace Quaer.Utils
{
    /// <summary>
    /// The Extensions class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks whether the specified element is on the web page.
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        /// <param name="by">The CSS selector</param>
        /// <param name="timeout">Waiting time for the web driver</param>
        /// <returns>A bool</returns>
        public static bool ElementIsPresent(this IWebDriver driver, By by, int timeout)
        {
            bool present = false;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            try
            {
                present = driver.FindElement(by).Displayed;
            }
            catch (NoSuchElementException)
            {
            }
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            return present;
        }

        /// <summary>
        /// Waits the web driver until the specific element will appear on the web page
        /// </summary>
        /// <param name="driver">Selenium web driver instance</param>
        /// <param name="by">The CSS selector</param>
        /// <param name="timeout">Waiting time for the web driver</param>
        /// <returns>A bool</returns>
        public static bool WaitUntilPresent(this IWebDriver driver, By by, int timeout = 10)
        {
            for (var i = 0; i < timeout; i++)
            {
                if (driver.ElementIsPresent(by, timeout)) return true;
                Thread.Sleep(1000);
            }
            return false;
        }

        /// <summary>
        /// Removes duplicate elements from the input sequence by key
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TKey">The key selector type</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="selector">The selector for enumerating</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            HashSet<TKey> seen = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seen.Add(selector(element)))
                    yield return element;
            }
        }

        /// <summary>
        /// Gets the string key from Resource.resx
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>The string value for the key</returns>
        public static string GetStringKey(string key)
        {
            return Properties.Resources.ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);
        }
    }
}
