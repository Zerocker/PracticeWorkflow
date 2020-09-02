using System.Windows;
using System.Windows.Controls;

namespace Juxta.Controls
{
    public partial class Table : UserControl
    {
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(Table), new PropertyMetadata(null));
        
        public Table()
        {
            InitializeComponent();
        }
    }
}
