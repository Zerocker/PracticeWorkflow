using ClosedXML.Excel;
using Juxta.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;

namespace Juxta.Services
{
    public class DummyService : IExcel
    {
        public DummyService()
        {
            Data = new List<Record>();
            Results = new ObservableCollection<Processed>();
            Left = new ObservableCollection<Processed>();
            Progress = 0d;
            Mainbook = null;
        }

        public XLWorkbook Mainbook { get; private set; }

        public List<Record> Data { get; private set; }

        public ObservableCollection<Processed> Results { get; set; }

        public ObservableCollection<Processed> Left { get; set; }

        public double Progress { get; private set; }

        public void Dispose() { }
        
        public void Iterate(DateTime? forced) { }

        public void LoadData(string path) { }

        public void LoadMain(string path) { }

        public void Save(string path, string sheetName, IEnumerable<Processed> list) { }

        public void SaveToMain(string path) { }
    }
}
