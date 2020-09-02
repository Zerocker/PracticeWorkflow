using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using Quaer.Utils;
using Quaer.Services;
using Quaer.Model;
using Quaer.Engine;

namespace Quaer.ViewModel
{
    using SelectItems = ObservableCollection<Selectable<IEngine>>;

    /// <summary>
    /// The viewmodel for main window
    /// </summary>
    public class MainViewModel : Observable
    {
        private IDialogService dialog;
        private IScraping scraping;
        private IProgress progress;

        private ICommand _updCmd;
        private ICommand _menuQuit;
        private ICommand _menuOB;
        private ICommand _menuCB;
        private ICommand _menuEB;
        private ICommand _about;

        private string _detected;
        private SelectItems _searchEngines;

        /// <summary>
        /// The property for detected browser version
        /// </summary>
        public string Detected
        {
            get => _detected;
            set => Set(ref _detected, value);
        }

        /// <summary>
        /// The property for selected search engines
        /// </summary>
        public SelectItems SearchEngines
        {
            get => _searchEngines;
            set => Set(ref _searchEngines, value);
        }

        /// <summary>
        /// The property for status bar info
        /// </summary>
        public IProgress StatusInfo
        {
            get => progress;
            set => OnPropertyChanged();
        }

        /// <summary>
        /// The command for getting selected search engines
        /// </summary>
        public ICommand UpdateEnginesCommand => _updCmd ??= new Command(() =>
        {
            scraping.Engines = _searchEngines.Where(x => x.IsSelected).Select(s => s.Object).ToList();
        });

        /// <summary>
        /// The command for quiting the program
        /// </summary>
        public ICommand MenuQuitCommand => _menuQuit ??= new Command(() =>
        {
            ChromeHelper.Driver.Quit();
            Application.Current.Shutdown();
        });

        /// <summary>
        /// The command for re-opening the browser via Selenium web driver
        /// </summary>
        public ICommand MenuOpenBrowserCommand => _menuOB ??= new Command(() =>
        {
            ChromeHelper.Driver.Quit();
            ChromeHelper.Initialize();
        });

        /// <summary>
        /// The command for closing the browser via Selenium web driver
        /// </summary>
        public ICommand MenuCloseBrowserCommand => _menuCB ??= new Command(() =>
        {
            ChromeHelper.Driver.Quit();
        });

        /// <summary>
        /// The command for editing the profile via browser itself
        /// </summary>
        public ICommand MenuEditBrowserCommand => _menuEB ??= new Command(() =>
        {
            ChromeHelper.Driver.Quit();
            ChromeHelper.OpenWithProfile();
            ChromeHelper.Initialize();
        });

        /// <summary>
        /// The command for displaying about dialog window
        /// </summary>
        public ICommand AboutCommand => _about ??= new Command(() =>
        {
            dialog.ShowInfo(Extensions.GetStringKey("MW_AboutInfo"));
        });

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialog">The dialog service</param>
        /// <param name="scraping">The scraping service</param>
        /// <param name="progress">The progress service</param>
        public MainViewModel(IDialogService dialog, IScraping scraping, IProgress progress)
        {
            this.scraping = scraping;
            this.progress = progress;
            this.dialog = dialog;
            
            SearchEngines = new SelectItems();
            Detected = ChromeHelper.DetectVersion();
            foreach (var engine in scraping.AvailableEngines)
            {
                SearchEngines.Add(new Selectable<IEngine>(engine, true, UpdateEnginesCommand));
            }
            
            scraping.Engines = scraping.AvailableEngines;
        }
    }
}