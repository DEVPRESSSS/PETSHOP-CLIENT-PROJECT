using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
using PetShop.WinForms.UpsertForms.Employee;
using PetShop.WinForms.UpsertForms.Products;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShop.UCpages
{
    /// <summary>
    /// Interaction logic for Employees.xaml
    /// </summary>
    public partial class Employees : UserControl
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private CollectionViewSource collectionViewSource;

        public Employees()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            collectionViewSource = new CollectionViewSource();

        }

        private void LoadAllEmployee()
        {

            sqlConnection.Open();
            string Query = "SELECT EmpNum, EmpName, EmpAdd, EmpDOB, EmpPhone, EmpEmail FROM EmployeeTbl";
            SqlCommand cmd = new SqlCommand(Query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<EmployeeModel> products = new List<EmployeeModel>();
            while (reader.Read())
            {
                products.Add(new EmployeeModel
                {
                    EmployeeId = (int)reader["EmpNum"],
                    Name = reader["EmpName"].ToString(),
                    Address = reader["EmpAdd"].ToString(),
                    DateCreated = Convert.ToDateTime(reader["EmpDOB"]).ToString("yyyy-MM-dd"),
                    Phone = reader["EmpPhone"].ToString(),
                    Gmail= reader["EmpEmail"].ToString()
                });
            }
            reader.Close();
            sqlConnection.Close();
            collectionViewSource.Source = products;
            EmployeeTable.ItemsSource = collectionViewSource.View;
          
        }
        private void editBtn_Click_1(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            if (btn == null) return;

            var selectedEmployee = btn.DataContext as EmployeeModel;

            if (selectedEmployee != null)
            {
                UpdateEmployee updateProduct = new UpdateEmployee
                {
                    DataContext = selectedEmployee
                };

                updateProduct.EmployeeUpdated += (s, e) => {

                    LoadAllEmployee();
                };

                updateProduct.ShowDialog();
            }

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            if (btn == null) return;
            var selectedEmployee = btn.DataContext as EmployeeModel;

            if(selectedEmployee != null)
            {

                MessageBoxResult result = MessageBox.Show(
                   "Are you sure you want to delete this product?",
                   "Confirmation",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand("delete from EmployeeTbl where EmpNum = @EmpKey", sqlConnection);
                        cmd.Parameters.AddWithValue("@EmpKey", selectedEmployee.EmployeeId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Employee deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        sqlConnection.Close();
                        LoadAllEmployee();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }

                }
            
            }
       
            
        }

        private void NewEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertEmployee insertProcduct = new InsertEmployee();

            insertProcduct.EmployeeUpdated += (s, e) => {

                LoadAllEmployee();
            };
            insertProcduct.ShowDialog();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllEmployee();
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (collectionViewSource.View != null)
            {
                collectionViewSource.View.Filter = item =>
                {
                    var employee = item as EmployeeModel;
                    if (employee == null) return false;

                    string searchText = SearchName.Text.ToLower();

                    return
                        employee.EmployeeId.ToString().Contains(searchText) ||
                        (!string.IsNullOrEmpty(employee.Name) && employee.Name.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(employee.Address) && employee.Address.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(employee.Phone) && employee.Phone.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(employee.Password) && employee.Password.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(employee.DateCreated) && employee.DateCreated.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(employee.Gmail) && employee.Gmail.ToLower().Contains(searchText));
                };
            }
        }

    }
}
