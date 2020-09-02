using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaer.Services
{
    /// <summary>
    /// Service for handling text files
    /// </summary>
    public class TxtFileService : IFileService<string>
    {
        /// <summary>
        /// Loads the list of strings from the text file
        /// </summary>
        /// <param name="filename">The path to the text file</param>
        /// <returns>The list of strings</returns>
        public IEnumerable<string> Open(string filename)
        {
            List<string> result = new List<string>();

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                using StreamReader sr = new StreamReader(fs, Encoding.UTF8, true, 1024);
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                        result.Add(line);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Saves the list of strings to the text file
        /// </summary>
        /// <param name="filename">The path to the text file</param>
        public void Save(string filename, IEnumerable<string> list)
        {
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                foreach (var item in list)
                {
                    sw.WriteLine(item);
                }
                
            }
        }
    }
}
