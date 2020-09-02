using System;
using System.Windows;
using Quaer.Services;
using Quaer.ViewModel;
using Quaer.Utils;

namespace Quaer
{
    /// <summary>
    /// The MainWindow logic class
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IDialogService dialog;
        readonly IFileService<string> file;
        readonly IScraping scraping;
        readonly IProgress progress;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            dialog = new DefaultDialogService();

            try
            {
                progress = new Model.StatusInfo();
                file = new TxtFileService();

                if (ChromeHelper.DetectVersion() != null)
                {
                    ChromeHelper.CheckProfile(dialog);
                    scraping = new ScrapingService(dialog, file);
                }
                else
                    throw new Exception("Google Chrome is not detected!");

                DataContext = new MainViewModel(dialog, scraping, progress);
                QueriesViewInstance.DataContext = new QueriesViewModel(dialog, file, scraping, progress);
                ResultsViewInstance.DataContext = new ResultsViewModel(dialog, file, scraping, progress);
            }
            catch (Exception ex)
            {
                dialog.ShowError(ex.Message);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Handles the  Menu/Quit event
        /// </summary>
        private void MainWindow_Quit(object sender, RoutedEventArgs e)
        {
            var main = this.DataContext as MainViewModel;
            var res = ResultsViewInstance.DataContext as ResultsViewModel;
            
            if (res.Results.Count > 0)
            {
                var result = dialog.ShowConfirm(Extensions.GetStringKey("MW_ConfimQuit"));
                if (result == MessageBoxResult.No)
                    return;
            }

            if ((main != null) && (main.MenuQuitCommand.CanExecute(null)))
                main.MenuQuitCommand.Execute(null);
        }

        /// <summary>
        /// Handles the  Menu/OpenQueries event
        /// </summary>
        private void OpenQueries_Click(object sender, RoutedEventArgs e)
        {
            var vm = QueriesViewInstance.DataContext as QueriesViewModel;   
            if ((vm != null) && (vm.LoadCommand.CanExecute(null)))
                vm.LoadCommand.Execute(null);
        }

        /// <summary>
        /// Handles the  Menu/SaveQueries event
        /// </summary>
        private void SaveQueries_Click(object sender, RoutedEventArgs e)
        {
            var vm = QueriesViewInstance.DataContext as QueriesViewModel;
            if ((vm != null) && (vm.SaveCommand.CanExecute(null)))
                vm.SaveCommand.Execute(null);
        }

        /// <summary>
        /// Handles the Menu/SaveResults event
        /// </summary>
        private void SaveResults_Click(object sender, RoutedEventArgs e)
        {
            var vm = ResultsViewInstance.DataContext as ResultsViewModel;
            if ((vm != null) && (vm.SaveCommand.CanExecute(null)))
                vm.SaveCommand.Execute(null);
        }
    }
}
