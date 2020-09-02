using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Juxta.ViewModels
{
    public class StatusBarViewModel : ReactiveObject
    {
        private string _message;
        private string _status;
        private TimeSpan _time = TimeSpan.Zero;
        private double _progress = 0;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public TimeSpan Time
        {
            get => _time;
            set => this.RaiseAndSetIfChanged(ref _time, value);
        }

        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public override string ToString()
        {
            return $"{Status} | {Message} | {Time} | {Progress}%";
        }
    }
}

