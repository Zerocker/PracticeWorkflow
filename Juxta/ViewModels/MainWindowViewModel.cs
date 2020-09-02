using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Juxta.Models;
using Juxta.Services;
using ReactiveUI;

namespace Juxta.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (value == 0)
                    StatusBar.Status = $"Кол-во: {Service.Results.Count}";
                else
                    StatusBar.Status = $"Кол-во: {Service.Left.Count}";
                this.RaiseAndSetIfChanged(ref _selectedTab, value);
            }
        }
        
        public DateTime PickedDate
        {
            get => _pickedDate;
            set => this.RaiseAndSetIfChanged(ref _pickedDate, value);
        }

        public bool Enabled
        {
            get => _enabled;
            set => this.RaiseAndSetIfChanged(ref _enabled, value);
        }

        public ObservableCollection<Processed> ProcessedResults
        {
            get => _processed;
            set => this.RaiseAndSetIfChanged(ref _processed, value);
        }

        public ObservableCollection<Processed> LeftResults
        {
            get => _left;
            set => this.RaiseAndSetIfChanged(ref _left, value);
        }

        public string MainFile
        {
            get => _mainFile;
            set => this.RaiseAndSetIfChanged(ref _mainFile, value);
        }

        public string DataFile
        {
            get => _dataFile;
            set => this.RaiseAndSetIfChanged(ref _dataFile, value);
        }

        public ReactiveCommand<Unit, Unit> Process_ { get; }
        public ReactiveCommand<Unit, Unit> Clear_ { get; }
        public ReactiveCommand<Unit, Unit> SaveTab_ { get; }
        public ReactiveCommand<Unit, Unit> Exit_ { get; }
        public ReactiveCommand<string, Unit> Open_ { get; }
        public ReactiveCommand<string, Unit> Close_ { get; }
        public ReactiveCommand<Unit, Unit> Save_ { get; }

        public StatusBarViewModel StatusBar;
        public IExcel Service;
        public IDialogService Dialog;

        private ObservableCollection<Processed> _processed;
        private ObservableCollection<Processed> _left;
        private DateTime _pickedDate;
        private bool _enabled;
        private string _mainFile;
        private string _dataFile;
        private int _selectedTab;

        private readonly Stopwatch _watch;
        private readonly CancellationTokenSource _tokenSource;
        
        public MainWindowViewModel()
        {
            Process_ = ReactiveCommand.CreateFromTask(Process);
            Clear_ = ReactiveCommand.Create(Clear);
            SaveTab_ = ReactiveCommand.Create(SaveTab);
            Exit_ = ReactiveCommand.Create(Exit);
            Open_ = ReactiveCommand.Create<string>(Open);
            Close_ = ReactiveCommand.Create<string>(Close);
            Save_ = ReactiveCommand.Create(Save);

            this.Service = new DummyService();
            this._pickedDate = DateTime.Today;
            this._enabled = false;
            this._tokenSource = new CancellationTokenSource();
            this._watch = new Stopwatch();
        }

        private async Task Process()
        { 
            var token = _tokenSource.Token;
            StatusBar.Message = $"Обрабатывается {Service.Data.Count} новых записей";

            try
            {
                Enabled = false;
                if (Service.Mainbook == null)
                    throw new Exception("Файл с рейсами не загружен!");
                
                var uiTask = Task.Run(() => UpdateUI(token), token);
                var mainTask = Task.Run(() =>
                {
                    Service.Iterate(PickedDate);
                    _tokenSource.Cancel();
                });

                await mainTask;
                await uiTask;
            }
            catch (Exception ex)
            {
                Dialog.ShowError(ex.Message);
            }
            finally
            {
                _tokenSource.Dispose();
                _watch.Stop();

                Enabled = true;
                StatusBar.Progress = 0d;
                StatusBar.Message = "Операция завершена!";
            }

            ProcessedResults = new ObservableCollection<Processed>(Service.Results);
            LeftResults = new ObservableCollection<Processed>(Service.Left);
        }

        private void UpdateUI(CancellationToken ct)
        {
            _watch.Start();
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    StatusBar.Progress = Service.Progress;
                    StatusBar.Time = _watch.Elapsed;
                });
            }
        }

        private void Clear()
        {
            if (Service.Results.Count > 0)
            {
                if (Dialog.ShowConfirm("Очистить текущую вкладку?")
                    == MessageBoxResult.No)
                    return;
            }

            if (SelectedTab == 0)
            {
                Service.Results.Clear();
                ProcessedResults.Clear();
            }

            if (SelectedTab == 1)
            {
                Service.Left.Clear();
                LeftResults.Clear();
            }
        }

        private void SaveTab()
        {
            try
            {
                if (Dialog.SaveFileDialog())
                {
                    Service.Results = new ObservableCollection<Processed>(ProcessedResults);
                    Service.Left = new ObservableCollection<Processed>(LeftResults);
                    
                    if (SelectedTab == 0)
                        Service.Save(Dialog.FilePath, "Книга 1", Service.Results);

                    if (SelectedTab == 1)
                        Service.Save(Dialog.FilePath, "Книга 1", Service.Left);

                    StatusBar.Message = $"Результаты из вкладки сохранены в {Path.GetFileName(Dialog.FilePath)}";
                }
            }
            catch (Exception Error)
            {
                Dialog.ShowError(Error.Message);
            }
        }

        private void Exit()
        {
            Service.Dispose();
            Application.Current.Shutdown();
        }

        private void Open(string obj)
        {
            try
            {
                if (Dialog.OpenFileDialog())
                {
                    if (obj == "main")
                    {
                        Service.Dispose();
                        if (Path.GetFileNameWithoutExtension(Dialog.FilePath).ToLower().Contains("общий список"))
                            Service = new ForeignService();
                        else
                            Service = new DomesticService();

                        Service.LoadMain(Dialog.FilePath);
                        MainFile = Path.GetFileName(Dialog.FilePath);
                        StatusBar.Message = $"Загружены данные с рейсами";
                    }

                    if (obj == "data")
                    {
                        if (Service.Mainbook == null)
                            throw new Exception("Файл с рейсами не открыт!");
                        
                        Service.LoadData(Dialog.FilePath);
                        DataFile = Path.GetFileName(Dialog.FilePath);
                        StatusBar.Message = $"Загружены новые данные: {Service.Data.Count}";
                        Enabled = true;
                    }       
                }
                else
                    StatusBar.Message = string.Empty;
            }
            catch (Exception Error)
            {
                Dialog.ShowError(Error.Message);
            }
        }

        private void Close(string obj)
        {
            if (Service.Results.Count > 0)
            {
                if (Dialog.ShowConfirm("Перед выходом список результатов будет очищен! Продолжить?")
                    == MessageBoxResult.No)
                    return;
            }

            StatusBar.Message = string.Empty;
            if (obj == "main")
            {
                Service.Mainbook.Dispose();
                ProcessedResults.Clear();
                LeftResults.Clear();
                MainFile = string.Empty;
            }
                
            if (obj == "data")
            {
                Service.Data.Clear();
                DataFile = string.Empty;
                Enabled = false;
            }
        }

        private void Save()
        {
            try
            {
                if (Dialog.SaveFileDialog())
                {
                   Service.Results = new ObservableCollection<Processed>(ProcessedResults);
                   Service.Left = new ObservableCollection<Processed>(LeftResults);

                   Service.SaveToMain(Dialog.FilePath);
                   StatusBar.Message = $"Данные сохранены в файле: {Dialog.FilePath}";
                }
                else
                    StatusBar.Message = string.Empty;
            }
            catch (Exception Error)
            {
                Dialog.ShowError(Error.Message);
            }
        }
    }
}
