using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaer.Services
{
    public interface IFileService<T>
    {
        IEnumerable<T> Open(string filename);
        void Save(string filename, IEnumerable<T> list);
    }
}
