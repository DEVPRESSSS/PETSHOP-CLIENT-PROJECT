using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PetShop.WinForms.UpsertForms.Products
{
    /// <summary>
    /// Interaction logic for InsertProcduct.xaml
    /// </summary>
    public partial class InsertProcduct : Window
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler ProductUpdated;
        public InsertProcduct()
        {
            InitializeComponent();

            sqlConnection= new SqlConnection(connection.ConnectionString);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateProduct();
        }

        private void CreateProduct()
        {
            if (!string.IsNullOrWhiteSpace(ProductName.Text) && !string.IsNullOrWhiteSpace(Price.Text)
                     && !string.IsNullOrWhiteSpace(Quantity.Text))
            {
                int qty = Convert.ToInt32(Quantity.Text);
                if (qty <= 0)
                {
                    MessageBox.Show("Qty cant be zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("insert into ProductTbl (PrName,PrCat,PrQty,PrPrice,CurrentStock) values(@PN,@PC,@PQ,@PP,@CS)", sqlConnection);
                    cmd.Parameters.AddWithValue("@PN", ProductName.Text);
                    cmd.Parameters.AddWithValue("@PC", (ProductCategory.SelectedItem as ComboBoxItem)?.Content.ToString());
                    cmd.Parameters.AddWithValue("@PQ", Convert.ToInt32(Quantity.Text));
                    cmd.Parameters.AddWithValue("@PP", Convert.ToInt32(Price.Text));
                    cmd.Parameters.AddWithValue("@CS", Convert.ToInt32(Quantity.Text));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    sqlConnection.Close();
                    ProductUpdated?.Invoke(this, EventArgs.Empty);

                    Clear();
                }
                catch (Exception Ex)
                {
                     MessageBox.Show(Ex.Message);

                }


            }
            else
            {

                MessageBox.Show("All fields are required", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Clear();
            }


        }

        private void Clear()
        {

            ProductName.Text = "";
            Price.Text = "";
            Quantity.Text = "";
            //ProductName.Text = "";
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.AllowOnlyNumbers(sender, e);
        }

        private void Quantity_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }
    }
}
