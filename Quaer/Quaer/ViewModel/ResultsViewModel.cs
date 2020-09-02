using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using Quaer.Model;
using Quaer.Services;
using Quaer.Utils;
using System.Threading;

namespace Quaer.ViewModel
{
    /// <summary>
    /// The viewmodel for results group box
    /// </summary>
    public class ResultsViewModel : Observable
    {
        public IDialogService dialog;
        public IScraping scraping;
        public IProgress progress;
        public IFileService<string> file;

        public bool _enabled = true;
        private ObservableCollection<Result> _results;
        private Result _selectedResult;
        public string _info;

        private int counter;
        private Stopwatch watch;
        private string status;
        private CancellationTokenSource tokenSource;

        private ICommand _startCmd;
        private ICommand _saveCmd;
        private ICommand _clearCmd;
        private ICommand _stopCmd;
        private ICommand _menuCpCmd;
        private ICommand _menuGoCmd;
        private ICommand _menuRmCmd;

        /// <summary>
        /// The property for enabling ResultsView
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => Set(ref _enabled, value);
        }

        /// <summary>
        /// The property for the list of displayed results
        /// </summary>
        public ObservableCollection<Result> Results
        {
            get => _results;
            set => Set(ref _results, value);
        }

        /// <summary>
        /// The property for a selected result item
        /// </summary>
        public Result SelectedResult
        {
            get => _selectedResult;
            set => Set(ref _selectedResult, value);
        }

        /// <summary>
        /// The command for searching queries and getting results
        /// <summary>
        public ICommand StartCommand => _startCmd ??= new Command(StartAsync);

        /// <summary>
        /// The command for saving the results list
        /// <summary>
        public ICommand SaveCommand => _saveCmd ??= new Command(Save);

        /// <summary>
        /// The command for clearing the results list
        /// <summary>
        public ICommand ClearCommand => _clearCmd ??= new Command(Clear);

        /// <summary>
        /// The command for stoping the searching operation
        /// <summary>
        public ICommand StopCommand => _stopCmd ??= new Command(() => StopAll(ref tokenSource));

        /// <summary>
        /// The command for coping the link from the selected result to clipboard
        /// <summary>
        public ICommand MenuCopyCommand => _menuCpCmd ??= new Command(MenuCopy);

        /// <summary>
        /// The command for opening the link from the selected result in the browser
        /// <summary>
        public ICommand MenuOpenCommand => _menuGoCmd ??= new Command(MenuOpen);

        /// <summary>
        /// The command for removing the selected result from the list
        /// <summary>
        public ICommand MenuRemoveCommand => _menuRmCmd ??= new Command(MenuRemove);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialog">The dialog service</param>
        /// <param name="scraping">The scraping service</param>
        /// <param name="progress">The progress service</param>
        /// <param name="file">The file service</param>
        public ResultsViewModel(IDialogService dialog, IFileService<string> file, 
            IScraping scraping, IProgress progress)
        {
            this.dialog = dialog;
            this.scraping = scraping;
            this.progress = progress;
            this.file = file;

            tokenSource = new CancellationTokenSource();
            Results = new ObservableCollection<Result>();
        }

        /// <summary>
        /// Async task for searching all queries
        /// </summary>
        /// <param name="ts">The source of CancellationToken</param>
        /// <param name="ts">The CancellationToken</param>
        /// <returns>Unique list of search results</returns>
        private IEnumerable<Result> RunSearch(ref CancellationTokenSource ts, CancellationToken ct)
        {
            var results = new List<Result>();
            var queries = scraping.Queries.Select(x => x.Object);

            foreach (string query in queries)
            {
                status = $"{Extensions.GetStringKey("Status_Processing")} {query}";
                results.AddRange(scraping.ProcessSearch(query));
                counter += 1;

                if (ct.IsCancellationRequested)
                    break;  
            }

            ts.Cancel();
            return results.DistinctBy(r => r.Link);
        }

        /// <summary>
        /// Updates UI while main task is running
        /// </summary>
        /// <param name="ts">The CancellationToken</param>
        private void UpdateUI(CancellationToken ct)
        {
            counter = 0;
            watch = Stopwatch.StartNew();
            
            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Enabled = false;
                    progress.Message = status;
                    progress.Progress = 100d * ((double)counter / (double)scraping.Queries.Count());
                    progress.Time = watch.Elapsed;
                });
            }

            watch.Stop();
            progress.Reset();
            Enabled = true;
        }

        /// <summary>
        /// Stops all async tasks
        /// </summary>
        /// <param name="ts">The source of CancellationToken</param>
        private void StopAll(ref CancellationTokenSource ts)
        {
            ts.Cancel();
            progress.Message = Extensions.GetStringKey("Status_StopAll");
            Enabled = true;
        }

        /// <summary>
        /// Logic for AddCommand
        /// </summary>
        private async void StartAsync()
        {
            tokenSource.Dispose();
            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            
            if (ChromeHelper.Driver == null)
                ChromeHelper.Initialize();

            try
            {
                if (scraping.Queries.Count < 1)
                    throw new Exception(Extensions.GetStringKey("RV_QueriesEmpty"));

                var uiTask = Task.Run(() => UpdateUI(token), token);
                var searchTask = Task.Run(() => RunSearch(ref tokenSource, token), token);

                Results = new ObservableCollection<Result>(await searchTask);
                await uiTask;
            }
            catch (Exception ex)
            {
                dialog.ShowError(ex.Message);
                return;
            }

            ChromeHelper.Driver.Navigate().GoToUrl("about:blank");
            ChromeHelper.Driver.Manage().Window.Minimize();
            progress.Message = Extensions.GetStringKey("Status_Done");
        }

        /// <summary>
        /// Logic for ClearCommand
        /// </summary>
        private void Clear()
        {
            var result = dialog.ShowConfirm(Extensions.GetStringKey("RV_ClearResults"));
            if (result == MessageBoxResult.Yes)
                Results.Clear();
        }

        /// <summary>
        /// Logic for SaveCommand
        /// </summary>
        private void Save()
        {
            if (dialog.SaveFileDialog())
            {
                var strings = Results.Select(x => x.ToString());
                file.Save(dialog.FilePath, strings);
            }
        }

        /// <summary>
        /// Logic for MenuCopyCommand
        /// </summary>
        private void MenuCopy()
        {
            Clipboard.SetText(SelectedResult.Link);
        }

        /// <summary>
        /// Logic for MenuOpenCommand
        /// </summary>
        private void MenuOpen()
        {
            ChromeHelper.Driver.Navigate().GoToUrl(SelectedResult.Link);
            ChromeHelper.Driver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Logic for MenuRemoveCommand
        /// </summary>
        private void MenuRemove()
        {
            Results.Remove(SelectedResult);
            SelectedResult = null;
        }
    }
}
