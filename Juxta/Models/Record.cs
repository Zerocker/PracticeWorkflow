using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Juxta.Models
{
    public class Record : ReactiveObject
    {
        private int _id;
        private string _surname;
        private string _name;
        private string _temp;
        private DateTime _checkDate;
        private ResultEnum _result;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public string Surname
        {
            get => _surname;
            set => this.RaiseAndSetIfChanged(ref _surname, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Temp
        {
            get => _temp;
            set => this.RaiseAndSetIfChanged(ref _temp, value);
        }

        public DateTime CheckDate
        {
            get => _checkDate;
            set => this.RaiseAndSetIfChanged(ref _checkDate, value);
        }

        public ResultEnum Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public Processed ToDisplayed(int id)
        {
            return new Processed()
            {
                Number = id,
                Fullname = $"{this.Name} {this.Surname}",
                Result = Result.ToString(),
                CheckDates = CheckDate.ToString(),
            };
        }

        public override string ToString()
        {
            return $"{Name} {Surname} => {Result} at {CheckDate}";
        }
    }
}
