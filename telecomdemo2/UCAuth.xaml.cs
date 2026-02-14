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

namespace telecomdemo2
{
    /// <summary>
    /// Логика взаимодействия для UCAuth.xaml
    /// </summary>
    public partial class UCAuth : UserControl
    {
        public UCAuth()
        {
            InitializeComponent();
            App.MainWindow.Height = 600;
            App.MainWindow.Width = 800;
            App.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            App.Controls.Add(this);
        }
        private void LoginButton_OnClick(object? sender, RoutedEventArgs e)
        {
            //User loginUser = Context.Connect.Users.FirstOrDefault(c => c.Login == LoginTextBox.Text && c.Password == PasswordTextBox.Text);
            if (LoginTextBox.Text != "")
            {
                //App.MainWindow.MainContentControl.Content = new MainUC(loginUser);
                //App.MainWindow.MainTextBlock.Text = loginUser.Fullname;
                App.MainWindow.MainContentControl.Content = new UCMain();
            }
            else
            {
                MessageBox.Show("Неверные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
