using System.Windows;

namespace Quaer.Services
{
    public interface IDialogService
    {
        string FilePath { get; set; }
        
        bool OpenFileDialog();
        
        bool SaveFileDialog();

        MessageBoxResult ShowConfirm(string message);
        
        MessageBoxResult ShowInfo(string message);
        
        MessageBoxResult ShowError(string message);

        MessageBoxResult ShowWarning(string message);
    }
}