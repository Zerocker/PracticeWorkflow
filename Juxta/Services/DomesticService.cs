using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ClosedXML.Excel;
using FuzzySharp;
using Juxta.Models;

namespace Juxta.Services
{
    public class DomesticService : IExcel
    {
        private IXLWorksheet Worksheet;

        public XLWorkbook Mainbook { get; private set; }

        public List<Record> Data { get; private set; }

        public ObservableCollection<Processed> Results { get; set; }

        public ObservableCollection<Processed> Left { get; set; }

        public double Progress { get; private set; }

        public DomesticService()
        {
            Data = new List<Record>();
            Results = new ObservableCollection<Processed>();
            Left = new ObservableCollection<Processed>();
        }

        public void LoadMain(string path)
        {
            Mainbook = new XLWorkbook(path);
            Worksheet = Mainbook.Worksheets.Worksheet(1);
        }

        public void LoadData(string path)
        {
            Data = new List<Record>();
            using (var book = new XLWorkbook(path))
            {
                var sheet = book.Worksheets.Worksheet(1);
                int length = sheet.RowsUsed().Count();
                
                // Exclude header from parsing
                for (int i = 2; i <= length; i++)
                {
                    int id = sheet.Cell(i, 1).GetValue<int>();
                    string surname = sheet.Cell(i, 2).GetString();
                    string name = sheet.Cell(i, 3).GetString();
                    string temp = sheet.Cell(i, 4).GetString();
                    DateTime check = sheet.Cell(i, 5).GetDateTime();
                    string result = sheet.Cell(i, 6).GetString();

                    Data.Add(new Record
                    {
                        Id = id,
                        Surname = surname,
                        Name = name,
                        Temp = temp,
                        Result = result.Parse(),
                        CheckDate = check.Date,
                    });
                }
            }
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
                Worksheet.Cell(item.Number, 2).Value = item.CheckDates;
            }
            
            Mainbook.SaveAs(path);
        }

        public void Iterate(DateTime? forced)
        {
            Progress = 0d;
            for (int i = 0; i < Data.Count; i++)
            {
                Record subject = Data[i];

                string actualName = (!subject.Name.Any(char.IsDigit) ? subject.Name : string.Empty);
                string formated = forced?.ToString("dd.MM.yy");
                string date = formated;
                if (forced == null)
                    date = subject.CheckDate.ToString("dd.MM.yy");

                subject.Name = (subject.Surname + " " + actualName);
                subject.Surname = string.Empty;
                subject.Temp = date;

                var result = Process(subject, false);
                if (result == null)
                    result = Process(subject, true);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result != null)
                        Results.Add(result);
                    else
                        Left.Add(Data[i].ToDisplayed(i + 1));
                });

                Progress = 100d * ((double)(i + 1) / (double)Data.Count);
            }
        }

        private Processed Process(Record record, bool direct)
        {
            static bool mainCheck(string input1, string input2)
                => (Fuzz.TokenSortRatio(input1, input2) > 70) && (Fuzz.TokenSetRatio(input1, input2) > 80);

            bool directCheck(string input1, string input2)
                => direct && (input1.Contains(input2));

            string name = record.Name.ToLower();
            IXLCell mainNameCell = Worksheet.CellsUsed(c => mainCheck(name, c.GetString().ToLower())
                                                    || directCheck(c.GetString().ToLower(), name)).FirstOrDefault();
            if (mainNameCell == null)
                return null;

            int rowNumber = mainNameCell.Address.RowNumber;
            var mainDateCell = Worksheet.Cell("B" + rowNumber);
            var mainResCell = Worksheet.Cell("A" + rowNumber);

            string mainDates = mainDateCell.GetString().ToLower();

            if (!mainDates.Contains(record.Temp))
                mainDates += ", " + record.Temp;

            mainDates = string.Join(",", mainDates.Split(',').Distinct());

            string mainResult = record.Result.ToString();
            if (mainResult == string.Empty)
                mainResult = mainResCell.GetString();

            if (mainResult.ToLower().Contains("двер") && mainResult.ToLower().Contains("соблюд"))
                mainDates += "-д.н.о.";

            if (mainResult.ToLower().Contains("соблюд") && mainResult.ToLower().Contains("двер"))
                mainDates += "-с.к.";

            return new Processed()
            { 
                Number = rowNumber, 
                Fullname = mainNameCell.GetString(),
                Result = mainResult,
                CheckDates = mainDates,
            };
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
