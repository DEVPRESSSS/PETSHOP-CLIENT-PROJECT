using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
using PetShop.UCpages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PetShop.WinForms.UpsertForms.Employee
{
    /// <summary>
    /// Interaction logic for UpdateEmployee.xaml
    /// </summary>
    public partial class UpdateEmployee : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler EmployeeUpdated;
        public UpdateEmployee()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }



        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        int employeeId;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var employee = DataContext as EmployeeModel;

            if (employee != null)
            {
                employeeId = employee.EmployeeId;
                EmpNameTb.Text = employee.Name;
                EmpPhoneTb.Text = employee.Phone;
                EmpAddTb.Text = employee.Address;
                Gmail.Text = employee.Gmail;


            }
        }
        private void Update()
        {

            if (EmpNameTb.Text == "" || EmpPhoneTb.Text == ""  || EmpAddTb.Text == "")
            {
                MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("Update EmployeeTbl set EmpName=@EN,EmpAdd=@EA,EmpDOB=@ED,EmpPhone=@EP,EmpEmail=@EmpEmail where EmpNum=@EKey", sqlConnection);
                    cmd.Parameters.AddWithValue("@EN", EmpNameTb.Text);
                    cmd.Parameters.AddWithValue("@EA", EmpAddTb.Text);
                    cmd.Parameters.AddWithValue("@ED", DateTime.Today);
                    cmd.Parameters.AddWithValue("@EP", EmpPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@EmpEmail", Gmail.Text);
                    cmd.Parameters.AddWithValue("@EKey", employeeId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Employee updated successfully","Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    sqlConnection.Close();

                    this.Close();
                  
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }
        private void EmpPhoneTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.AllowOnlyNumbers(sender, e);

        }

        private void EmpPhoneTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }

        private void PasswordTb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }

        private void Gmail_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }

        private void Gmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.EmailTextComposition(sender, e);
        }

        private void EmpNameTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.PersonNameTextComposition(sender, e);

        }
    }
}
