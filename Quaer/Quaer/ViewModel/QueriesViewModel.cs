using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Quaer.Model;
using Quaer.Services;
using Quaer.Utils;
using Quaer.Views;

namespace Quaer.ViewModel
{
    /// <summary>
    /// The viewmodel for queries group box
    /// </summary>
    public class QueriesViewModel : Observable
    {
        private IScraping scraping;
        private IProgress progress;
        private IDialogService dialog;
        private IFileService<string> file;

        private Selectable<string> _selected;

        private ICommand _addCmd;
        private ICommand _ldrCmd;
        private ICommand _rmvCmd;
        private ICommand _selCmd;
        private ICommand _unsCmd;
        private ICommand _delCmd;
        private ICommand _saveCmd;

        /// <summary>
        /// The property for the list of displayed queries
        /// </summary>
        public ObservableCollection<Selectable<string>> Queries
        {
            get => scraping.Queries;
            set
            {
                scraping.Queries = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The property for a selected query item
        /// </summary>
        public Selectable<string> Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        /// <summary>
        /// The command for adding an empty query to list
        /// <summary>
        public ICommand AddCommand => _addCmd ??= new Command(Add);

        /// <summary>
        /// The command for loading a new queries from the text file
        /// </summary>
        public ICommand LoadCommand => _ldrCmd ??= new Command(Load);
        
        /// <summary>
        /// The command for removing selected query from the list
        /// </summary>

        public ICommand RemoveCommand => _rmvCmd ??= new Command<Selectable<string>>(Remove);

        /// <summary>
        /// The command for selecting all queries in the list
        /// </summary>
        public ICommand SelectCommand => _selCmd ??= new Command(() => SelectAll(true));

        /// <summary>
        /// The command for unselecting all queries in the list
        /// </summary>
        public ICommand UnselectCommand => _unsCmd ??= new Command(() => SelectAll(false));

        /// <summary>
        /// The command for clearing all queries
        /// </summary>
        public ICommand DeleteCommand => _delCmd ??= new Command(Delete);

        /// <summary>
        /// The command for saving current queries list
        /// </summary>
        public ICommand SaveCommand => _saveCmd ??= new Command(Save);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialog">The dialog service</param>
        /// <param name="scraping">The scraping service</param>
        /// <param name="progress">The progress service</param>
        /// <param name="file">The file service</param>
        public QueriesViewModel(IDialogService dialog, IFileService<string> file, 
            IScraping scraping, IProgress progress)
        {
            this.scraping = scraping;
            this.progress = progress;
            this.dialog = dialog;
            this.file = file;
            Queries = new ObservableCollection<Selectable<string>>();
        }

        /// <summary>
        /// Logic for SelectCommand
        /// </summary>
        /// <param name="value">A bool value</param>
        private void SelectAll(bool value)
        {
            foreach (var item in Queries)
                item.IsSelected = value;
        }

        /// <summary>
        /// Logic for RemoveCommand
        /// </summary>
        /// <param name="value">The list of queries</param>
        private void Remove(Selectable<string> selectable)
        {
            if (selectable != null)
            {
                Queries.Remove(selectable);
                Count();
            }
        }

        /// <summary>
        /// Logic for AddCommand
        /// </summary>
        private void Add()
        {
            Selectable<string> item = new Selectable<string>();
            Queries.Insert(0, item);
            Selected = item;
            Count();
        }

        /// <summary>
        /// Logic for LoadCommand
        /// </summary>
        private void Load()
        {
            var newData = scraping.LoadQueries();
            foreach (var item in newData)
            {
                Queries.Add(new Selectable<string>(item));
            }
            Count();
        }

        /// <summary>
        /// Logic for DeleteCommand
        /// </summary>
        private void Delete()
        {
            var remaining = Queries.ToList();
            remaining.RemoveAll(q => q.IsSelected);
            Queries = new ObservableCollection<Selectable<string>>(remaining);
            Count();
        }

        /// <summary>
        /// Logic for SaveCommand
        /// </summary>
        private void Save()
        {
            if (dialog.SaveFileDialog())
                file.Save(dialog.FilePath, Queries.Select(x => x.Object));
        }

        /// <summary>
        /// Counts the list of queries
        /// </summary>
        private void Count()
        {
            progress.Status = Extensions.GetStringKey("Status_Total") + " " + Queries.Count;
        }
    }
}