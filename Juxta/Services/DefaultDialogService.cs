using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Juxta.Services
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            string init = (FilePath == null)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Path.GetDirectoryName(FilePath);

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = init,
                Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            string init = (FilePath == null)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : Path.GetDirectoryName(FilePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = init,
                Filter = "Файлы Excel (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
            };
            
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public MessageBoxResult ShowConfirm(string message)
        {
            return MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }

        public MessageBoxResult ShowInfo(string message)
        {
            return MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult ShowError(string message)
        {
            return MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult ShowWarning(string message)
        {
            return MessageBox.Show(message, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        }
    }
}
