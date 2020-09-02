using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;

namespace Juxta.Models
{
    public class Processed : ReactiveObject
    {
        private int _number;
        private string _fullname;
        private string _result;
        private string _checkDates;

        public Processed() { }

        public Processed(int number, string fullname, string result, string checkDates)
        {
            Number = number;
            Fullname = fullname;
            Result = result;
            CheckDates = checkDates;
        }

        public int Number
        {
            get => _number;
            set => this.RaiseAndSetIfChanged(ref _number, value);
        }

        public string Fullname
        {
            get => _fullname;
            set => this.RaiseAndSetIfChanged(ref _fullname, value);
        }

        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public string CheckDates
        {
            get => _checkDates;
            set => this.RaiseAndSetIfChanged(ref _checkDates, value);
        }

        public override string ToString()
        {
            return $"{Fullname} => {Result} от {CheckDates}";
        }
    }
}
