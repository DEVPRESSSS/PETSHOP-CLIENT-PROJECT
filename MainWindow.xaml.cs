using PetShop.Authentication;
using PetShop.UCpages;
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

namespace PetShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentArea.Content = new Products();

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Products();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {

            if(WindowState== WindowState.Normal)
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

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Employees();

        }

        private void Billing_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Billing();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Sales_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new Sale();

        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {

                var login = new Login();
                login.Show();

                Window.GetWindow(this)?.Close();
            }
        }
    }
}