using System;
using System.Windows;
using Quaer.Services;

namespace Quaer
{
    /// <summary>
    /// The Application class
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main Thread
        /// </summary>
        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.Run();

            // Quits the web driver even if MenuQuitCommand was called
            ChromeHelper.Driver.Quit();
        }
    }
}
