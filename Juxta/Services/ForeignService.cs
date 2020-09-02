using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClosedXML.Excel;
using FuzzySharp;
using Juxta.Models;

namespace Juxta.Services
{
    public class ForeignService : IExcel
    {
        private IXLWorksheet Worksheet;

        public XLWorkbook Mainbook { get; private set; }

        public List<Record> Data { get; private set; }

        public ObservableCollection<Processed> Results { get; set; }
        
        public ObservableCollection<Processed> Left { get; set; }

        public double Progress { get; private set; }

        public ForeignService()
        {
            Data = new List<Record>();
            Results = new ObservableCollection<Processed>();
            Left = new ObservableCollection<Processed>();
        }

        public void LoadData(string path)
        {
            Data = new List<Record>();
            using (var book = new XLWorkbook(path))
            {
                var sheet = book.Worksheets.Worksheet(1);
                int length = sheet.RowsUsed().Count();

                for (int i = 2; i <= length; i++)
                {
                    int row = sheet.Cell(i, 1).GetValue<int>();
                    string[] names = sheet.Cell(i, 2).GetString().Split(' ');
                    string result = sheet.Cell(i, 3).GetString();
                    string checkDates = sheet.Cell(i, 4).GetString();

                    Data.Add(new Record
                    {
                        Id = row,
                        Name = $"{names[1]} {names[2]}",
                        Surname = names[0],
                        Temp = checkDates,
                        Result = Extensions.Parse(result)
                    });
                }
            }
        }

        public void LoadMain(string path)
        {
            Mainbook = new XLWorkbook(path);
            Worksheet = Mainbook.Worksheets.Worksheet(1);
        }

        public void Iterate(DateTime? forced)
        {
            Progress = 0d;
            Results.Clear();
            Left.Clear();

            for (int i = 0; i < Data.Count; i++)
            {
                Record subject = Data[i];
                string date = forced?.ToString("dd.MM.yy");

                if (subject.Temp == string.Empty)
                    subject.Temp = date;

                var result = Process(subject, false);
                if (result == null)
                    result = Process(subject, true);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result != null)
                        Results.Add(result);
                    else
                    {
                        var left = new Processed()
                        {
                            Number = i+1,
                            Fullname = $"{Data[i].Surname} {Data[i].Name}",
                            Result = $"{Data[i].Result}, {Data[i].Temp}",
                            CheckDates = null,
                        };
                        Left.Add(left);
                    }  
                });

                Progress = 100d * ((double)(i + 1) / (double)Data.Count);
            }
        }

        private Processed Process(Record record, bool direct)
        {
            static bool mainCheck(string input1, string input2)
                => (Fuzz.TokenSortRatio(input1, input2) == 100) && (Fuzz.TokenSetRatio(input1, input2) == 100);

            bool directCheck(string input1, string input2)
                => direct && (input1.Contains(input2));

            string surname = record.Surname.ToUpper();
            string name = record.Name.ToUpper();
            
            IXLCell mainNameCell = Worksheet
                .CellsUsed(c => mainCheck(name, c.GetString().ToUpper()) || directCheck(c.GetString().ToUpper(), name))
                .FirstOrDefault();
            
            IXLCell mainSurnameCell = Worksheet
                .CellsUsed(c => mainCheck(surname, c.GetString().ToUpper()) || directCheck(c.GetString().ToUpper(), surname))
                .FirstOrDefault();
            
            if ((mainNameCell == null) || (mainSurnameCell) == null)
                return null;

            int rowName = mainNameCell.Address.RowNumber;
            int rowSurname = mainSurnameCell.Address.RowNumber;

            if (rowName != rowSurname)
                return null;

            var mainResCell = Worksheet.Cell("A" + rowName);
            string mainResult = mainResCell.GetString();

            if ((int)record.Result == (int)mainResult.Parse())
                mainResult += ", " + record.Temp;
            if (mainResult == string.Empty)
                mainResult += $"{record.Result}, {record.Temp}";

            if (mainResult.ToLower().Contains("двер") && mainResult.ToLower().Contains("соблюд"))
                mainResult += "-д.н.о.";

            if (mainResult.ToLower().Contains("соблюд") && mainResult.ToLower().Contains("двер"))
                mainResult += "-с.к.";

            var mainDateCell = Worksheet.Cell("U" + rowName);
            var mainDate = record.Temp;
            if (mainDateCell.Value as string != string.Empty)
                mainDate = mainDateCell.GetDateTime().ToString("dd.MM.yy");

            return new Processed()
            {
                Number = rowName,
                Fullname = mainNameCell.GetString(),
                Result = mainResult,
                CheckDates = mainDate,
            };
        }

        public void Save(string path, string sheetName, IEnumerable<Processed> list)
        {
            var headers = new string[] { "Строка", "Ф.И.О.", "Результат", "Даты обхода" };

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, 1 + i).Value = headers[i];

            var array = list.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                ws.Cell(i + 2, 1).Value = array[i].Number;
                ws.Cell(i + 2, 2).Value = array[i].Fullname;
                ws.Cell(i + 2, 3).Value = array[i].Result;
                ws.Cell(i + 2, 4).Value = array[i].CheckDates;
            }

            wb.SaveAs(path);
        }

        public void SaveToMain(string path)
        {
            for (int i = 0; i < Results.Count; i++)
            {
                var item = Results[i];
                Worksheet.Cell(item.Number, 1).Value = item.Result;
                Worksheet.Cell(item.Number, 21).Value = item.CheckDates;
            }

            Mainbook.SaveAs(path);
        }

        public void Dispose()
        {
            if (Mainbook != null)
            {
                Data.Clear();
                Left.Clear();
                Results.Clear();
                Worksheet = null;
                Mainbook.Dispose();
            }
        }
    }
}
