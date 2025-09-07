using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
using PetShop.UCpages;
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

namespace PetShop.WinForms.UpsertForms.Products
{
    /// <summary>
    /// Interaction logic for StockIn.xaml
    /// </summary>
    public partial class StockIn : Window
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        public event EventHandler ProductUpdated;

        public StockIn()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

        }



        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        int productId = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var product = DataContext as ProductModel;

            productId = product.ProductId;
         

        }

        private void AddStock()
        {
            if (!string.IsNullOrWhiteSpace(Qty.Text))
            {
                int qty = Convert.ToInt32(Qty.Text);

                if (qty <= 0)
                {
                    MessageBox.Show("Qty cant be zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand(@"UPDATE ProductTbl
                            SET PrQty = PrQty + @Qty,
                                CurrentStock = CurrentStock + @CurrentStock
                            WHERE PrId = @PKey;
                            ", sqlConnection);
                    cmd.Parameters.AddWithValue("@Qty", Qty.Text);
                    cmd.Parameters.AddWithValue("@CurrentStock", Qty.Text);
                    cmd.Parameters.AddWithValue("@PKey", productId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product Updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);



                    this.Close();

                    ProductUpdated?.Invoke(this, EventArgs.Empty);
                    sqlConnection.Close();

                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Fields cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


            }

        }

        private void AddStockBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStock();
        }

        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Qty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void Qty_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
