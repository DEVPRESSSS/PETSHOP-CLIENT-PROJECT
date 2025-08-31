using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.UCpages;
using PetShop.UCpages.Cashier;
using System;
using System.Collections.Generic;
using System.Data;
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
using static Azure.Core.HttpHeader;

namespace PetShop.Authentication
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public Login()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

        private void Passwordtxt_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Passwordtxt.Password.Length > 0)
            {
                Eye.Visibility = Visibility.Visible;
                PasswordErrorMessage.Text = "";
                Passwordtxt.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#727272"));
                Passwordtxt.BorderThickness = new Thickness(0, 0, 0, 2);
            }
            else
            {
                Eye.Visibility = Visibility.Collapsed;
            }

        }

        private void Usernametxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Usernametxt.Text.Length > 0)
            {

                UsernameErrorMessage.Text = "";
                Usernametxt.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#727272"));
                Usernametxt.BorderThickness = new Thickness(0, 0, 0, 2);

            }

        }

        private void PasswordUnmask_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordUnmask.Text.Length == 0)
            {

                Eye2.Visibility = Visibility.Hidden;


            }
            else
            {

                Eye2.Visibility = Visibility.Visible;

            }

        }

        private void forgotpassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ForgotPassword forgotPassword = new ForgotPassword();

            //call the show method to show the form
            forgotPassword.Show();

            //close the login form
            this.Close();

        }


        private void ClearTextBoxes()
        {

            //Cleart the username textbox
            Usernametxt.Text = "";

            //Clear the password box
            Passwordtxt.Password = "";

            ////Set the passwordUnmask to hidden
            //PasswordUnmask.Visibility = Visibility.Hidden;

            ////Set the passwordtxt to be visible
            //Passwordtxt.Visibility = Visibility.Visible;


        }

        private void Eye_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Passwordtxt.Visibility == Visibility.Visible)
            {
                Eye.Visibility = Visibility.Collapsed;
                Eye2.Visibility = Visibility.Visible;

                PasswordUnmask.Text = Passwordtxt.Password;
                PasswordUnmask.Visibility = Visibility.Visible;
                Passwordtxt.Visibility = Visibility.Hidden;

            }
            else
            {

                PasswordUnmask.Visibility = Visibility.Collapsed;

                Passwordtxt.Visibility = Visibility.Visible;

            }

        }

        private void Eye2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PasswordUnmask.Visibility == Visibility.Visible)
            {
                Eye.Visibility = Visibility.Visible;
                Eye2.Visibility = Visibility.Collapsed;

                Passwordtxt.Password = PasswordUnmask.Text;
                Passwordtxt.Visibility = Visibility.Visible;
                PasswordUnmask.Visibility = Visibility.Hidden;

            }

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Auth();
        }

        private void Auth()
        {
            // Use whichever textbox is currently visible for password
            string username = Usernametxt.Text.Trim();
            string password = Passwordtxt.Visibility == Visibility.Visible
                                ? Passwordtxt.Password
                                : PasswordUnmask.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill all fields", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ClearTextBoxes();
                return;
            }

            try
            {
                using (sqlConnection = new SqlConnection(connection.ConnectionString))
                {
                    sqlConnection.Open();

                    string query = @"SELECT EmpRole 
                             FROM EmployeeTbl 
                             WHERE EmpName = @username AND EmpPass = @password";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        var role = cmd.ExecuteScalar() as string;

                        if (!string.IsNullOrEmpty(role))
                        {
                            string user = username; 

                            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                            {
                                MainWindow main = new MainWindow();
                                main.Show();
                                this.Close();
                            }
                            else
                            {
                                // Go to Cashier Window
                                CashierDashboard cashier = new CashierDashboard(user);
                                cashier.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ClearTextBoxes();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ClearTextBoxes();
            }
        }

        private void Usernametxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.UsernameTextComposition(sender, e);

        }

        private void Usernametxt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           HelperValidation.InputValidator.Username(sender,e);

        }
    }
}
