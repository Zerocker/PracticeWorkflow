using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Quaer.Services;
using Quaer.Utils;

namespace Quaer.Services
{
    /// <summary>
    /// Service for handling dialog windows
    /// </summary>
    public class DefaultDialogService : IDialogService
    {
        /// <summary>
        /// The user-selected file path using FileDialog
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Opens file dialog window for loading
        /// </summary>
        /// <returns>User successfully selected the path for loading</returns>
        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = Extensions.GetStringKey("Dialog_Txt") + " (*.txt) | *.txt"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens file dialog window for saving
        /// </summary>
        /// <returns>User successfully selected the path for saving</returns>
        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                RestoreDirectory = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = Extensions.GetStringKey("Dialog_Txt") + " (*.txt) | *.txt"
            };
            
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Displays confirm dialog window
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <returns>MessageBoxResult object</returns>
        public MessageBoxResult ShowConfirm(string message)
        {
            return MessageBox.Show(message, Extensions.GetStringKey("Dialog_Confirm"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Displays information dialog window
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <returns>MessageBoxResult object</returns>
        public MessageBoxResult ShowInfo(string message)
        {
            return MessageBox.Show(message, Extensions.GetStringKey("Dialog_Info"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Displays error dialog window
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <returns>MessageBoxResult object</returns>
        public MessageBoxResult ShowError(string message)
        {
            return MessageBox.Show(message, Extensions.GetStringKey("Dialog_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays warning dialog window
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <returns>MessageBoxResult object</returns>
        public MessageBoxResult ShowWarning(string message)
        {
            return MessageBox.Show(message, Extensions.GetStringKey("Dialog_Warning"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }
    }
}
