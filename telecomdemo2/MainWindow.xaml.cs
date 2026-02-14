using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace telecomdemo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;
            App.Controls = new List<UserControl>();

            App.MainWindow.MainContentControl.Content = new UCAuth();
        }
        private void BackButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (App.Controls.Count > 1)
            {
                App.Controls.RemoveAt(App.Controls.Count - 1);
                App.MainWindow.MainContentControl.Content = App.Controls[App.Controls.Count - 1];
            }
        }
    }
}