using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaer.Services
{
    public interface IProgress
    {
        bool IsWorking { get; set; }

        string Status { get; set; }

        string Message { get; set; }

        double Progress { get; set; }

        TimeSpan Time { get; set; }

        void Reset();
    }
}
