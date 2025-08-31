using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
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

namespace PetShop.WinForms.UpsertForms.Employee
{
    /// <summary>
    /// Interaction logic for InsertEmployee.xaml
    /// </summary>
    public partial class InsertEmployee : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler EmployeeUpdated;
        public InsertEmployee()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }

    

        private void CloseBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(EmpNameTb.Text) && !string.IsNullOrWhiteSpace(EmpAddTb.Text)
                    && !string.IsNullOrWhiteSpace(EmpPhoneTb.Text) && !string.IsNullOrWhiteSpace(PasswordTb.Text) && !string.IsNullOrWhiteSpace(Gmail.Text))
                {

                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("insert into EmployeeTbl (EmpName,EmpAdd,EmpDOB,EmpPhone,EmpPass,EmpEmail) values(@EN,@EA,@ED,@EP,@EPa,@EmpEmail)", sqlConnection);
                    cmd.Parameters.AddWithValue("@EN", EmpNameTb.Text);
                    cmd.Parameters.AddWithValue("@EA", EmpAddTb.Text);
                    cmd.Parameters.AddWithValue("@ED", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EP", EmpPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@EPa", PasswordTb.Text);
                    cmd.Parameters.AddWithValue("@EmpEmail", Gmail.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Employee added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    sqlConnection.Close();
                    ClearAll();
                    EmployeeUpdated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("All fields are required ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ClearAll();
                }


            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void ClearAll()
        {
            EmpNameTb.Text = "";
            EmpAddTb.Text = "";
            EmpPhoneTb.Text = "";
            PasswordTb.Text = "";
            Gmail.Text = "";

        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

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
