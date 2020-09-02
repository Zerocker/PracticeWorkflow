using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quaer.Utils
{
    public static class DebugData
    {
        public static string Executable = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string Queries = Path.Combine(Executable, "queries.txt");
    }
}
