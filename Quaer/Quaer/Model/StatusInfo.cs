using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quaer.Services;
using Quaer.Utils;

namespace Quaer.Model
{
    /// <summary>
    /// Class for displaying information in the status bar
    /// </summary>
    public class StatusInfo : Observable, IProgress
    {
        private bool _isWorking = false;
        private string _status = string.Empty;
        private string _message = string.Empty;
        private double _progress = 0d;
        private TimeSpan _time = TimeSpan.Zero;

        /// <summary>
        /// Whether a task is being completed
        /// </summary>
        public bool IsWorking
        {
            get => _isWorking;
            set => Set(ref _isWorking, value);
        }

        /// <summary>
        /// The status string
        /// </summary>
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        /// <summary>
        /// The message string
        /// </summary>
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        /// <summary>
        /// The value of the progress bar
        /// </summary>
        public double Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }

        /// <summary>
        /// The current time when a task is completed
        /// </summary>
        public TimeSpan Time
        {
            get => _time;
            set => Set(ref _time, value);
        }

        /// <summary>
        /// Resets all fields of the object of class
        /// </summary>
        public void Reset()
        {
            IsWorking = false;
            Message = string.Empty;
            Progress = 0;
            Time = TimeSpan.Zero;
        }
    }
}
