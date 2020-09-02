using Juxta.Services;
using Juxta.ViewModels;
using System.Windows;

namespace Juxta.Views
{
    public partial class MainWindow : Window
    {
        private readonly StatusBarViewModel statusBarVM;
        private readonly MainWindowViewModel mainVM;
        private readonly IDialogService dialogService;

        public MainWindow()
        {
            InitializeComponent();
            dialogService = new DefaultDialogService();

            statusBarVM = new StatusBarViewModel();
            mainVM = new MainWindowViewModel()
            {
                Dialog = dialogService,
                StatusBar = statusBarVM
            };

            this.DataContext = mainVM;
            this.StatusBarInstance.DataContext = statusBarVM;
        }
    }
}
