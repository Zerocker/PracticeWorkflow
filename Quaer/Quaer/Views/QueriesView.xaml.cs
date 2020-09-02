using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Quaer.Services;
using Quaer.ViewModel;

namespace Quaer.Views
{
    public partial class QueriesView : UserControl
    {
        public QueriesView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                if (sender is DataGrid grid && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    if (!dgr.IsMouseOver)
                    {
                        dgr.IsSelected = false;
                    }
                }
            }
        }
    }
}
