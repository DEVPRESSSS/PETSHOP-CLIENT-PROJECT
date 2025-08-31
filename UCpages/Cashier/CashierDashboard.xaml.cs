using PetShop.Authentication;
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
using System.Windows.Shapes;

namespace PetShop.UCpages.Cashier
{
    /// <summary>
    /// Interaction logic for CashierDashboard.xaml
    /// </summary>
    public partial class CashierDashboard : Window
    {

        private string _user;
        public CashierDashboard(string user)
        {
            InitializeComponent();
            _user= user;
            MainContentArea.Content = new BillinCashier(_user);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                
                var login = new Login();
                login.Show();

                Window.GetWindow(this)?.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;

            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }
    }
}
