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
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;

namespace telecom_demo
{
    /// <summary>
    /// Логика взаимодействия для MainUC.xaml
    /// </summary>
    public partial class MainUC : UserControl
    {
        public MainUC()
        {
            InitializeComponent();
            App.Controls.Add(this);
        }
        private void OrderButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new OrderUC();
        }
        private void ProductButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new ProductUC();
        }
        private void ReportButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new ReportUC();
        }
        private void TestingButton_OnClick(object? sender, RoutedEventArgs e)
        {
            App.MainWindow.MainContentControl.Content = new TestingUC();
        }
        private void SortButton_OnChecked(object? sender, RoutedEventArgs e)
        {
        }
    }
}
