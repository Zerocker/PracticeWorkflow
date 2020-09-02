using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ClosedXML.Excel;
using Juxta.Models;

namespace Juxta.Services
{
    public interface IExcel : IDisposable
    {
        XLWorkbook Mainbook { get; }

        List<Record> Data { get; }

        ObservableCollection<Processed> Results { get; set; }

        ObservableCollection<Processed> Left { get; set; }

        double Progress { get; }

        void LoadMain(string path);
       
        void LoadData(string path);

        void Iterate(DateTime? forced);

        void Save(string path, string sheetName, IEnumerable<Processed> list);
        
        void SaveToMain(string path);
    }
}
