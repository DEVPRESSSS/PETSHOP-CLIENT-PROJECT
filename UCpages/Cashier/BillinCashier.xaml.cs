using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
using PetShop.WinForms.UpsertForms;
using PetShop.WinForms.UpsertForms.Receipt;
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

namespace PetShop.UCpages.Cashier
{
    /// <summary>
    /// Interaction logic for BillinCashier.xaml
    /// </summary>
    public partial class BillinCashier : UserControl
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private List<CartModel> cartModels = new List<CartModel>();

        private string _user;
        public BillinCashier(string user)
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            _user = user;

            
        }
        private void DisplayAllProducts()
        {
            sqlConnection.Open();
            string Query = "SELECT PrId, PrName, PrCat, PrQty, PrPrice, CurrentStock FROM ProductTbl";
            SqlCommand cmd = new SqlCommand(Query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<ProductModel> products = new List<ProductModel>();
            while (reader.Read())
            {
                products.Add(new ProductModel
                {
                    ProductId = (int)reader["PrId"],
                    ProductName = reader["PrName"].ToString(),
                    Category = reader["PrCat"].ToString(),
                    Quantity = (int)reader["PrQty"],
                    CurrentStock = (int)reader["CurrentStock"],
                    Price = (int)reader["PrPrice"]
                });
            }
            reader.Close();
            sqlConnection.Close();

            ProductTable.ItemsSource = products;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var delete = sender as Button;
            if (delete == null) return;
            var selectedCart = delete.DataContext as CartModel;

            if (selectedCart != null)
            {

                RefundStock();

                cartModels.Remove(selectedCart);
                CartTable.Items.Refresh();
                CalculateGrandTotal();
                DisableAmountTextBox();

            }
        }

        private void CheckOutBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckOut();
            ReceiptTemplate();
        }
        decimal change = 0;
        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal grandTotal = Convert.ToDecimal(GrandTotalText.Text);
            if (AmountTextBox.Text == "")
            {
                ChangeText.Text = "";
            }

            if (decimal.TryParse(AmountTextBox.Text, out decimal amountPaid))
            {

                if (amountPaid < grandTotal)
                {

                    CheckOutBtn.Cursor = Cursors.No;
                    CheckOutBtn.IsEnabled = false;

                    return;
                }
                else
                {

                    CheckOutBtn.IsEnabled = true;
                    CheckOutBtn.Cursor = Cursors.Hand;
                    //Calculate change
                    change = amountPaid - grandTotal;
                    ChangeText.Text = change.ToString("N2");
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayAllProducts();
            CartTable.ItemsSource = cartModels;
            CalculateGrandTotal();

            //Disable checkout button
            CheckOutBtn.Cursor = Cursors.No;
            CheckOutBtn.IsEnabled = false;

            DisableAmountTextBox();
        }

        private void ProductTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the DataGrid that triggered the event
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            var selectedProduct = dataGrid.SelectedItem as ProductModel;

            if (selectedProduct != null)
            {
                AddToBill addToBill = new AddToBill
                {
                    DataContext = selectedProduct
                };

                addToBill.AddedToCartEvent += (s, ev) =>
                {
                    var newItem = s as CartModel;
                    if (newItem != null)
                    {
                        AddToCart(newItem);
                    }
                };



                addToBill.ShowDialog();



            }
        }

        public void AddToCart(CartModel item)
        {


            cartModels.Add(item);

            CartTable.Items.Refresh();
            CalculateGrandTotal();
            UpdateStock();
            DisableAmountTextBox();
        }

        private void CalculateGrandTotal()
        {
            var cartItems = CartTable.ItemsSource as IEnumerable<CartModel>;
            if (cartItems == null) return;

            decimal grandTotalValue = 0;

            foreach (var item in cartItems)
            {
                grandTotalValue += item.SubTotal;
            }

            GrandTotalText.Text = grandTotalValue.ToString("N2");
        }

        private void CheckOut()
        {
            var cartItems = CartTable.ItemsSource as IEnumerable<CartModel>;
            if (cartItems == null) return;

            string BillId = $"{Guid.NewGuid().ToString().Substring(0, 6)}";

            try
            {
                sqlConnection.Open();

                foreach (var item in cartItems)
                {
                    string query = @"INSERT INTO [Transaction] 
                (BillId, ProductName, Quantity, AmountPaid, Change, CashierName, DateOfTransaction, Price, SubTotal, GrandTotal) 
                VALUES (@BillId, @ProductName, @Quantity, @AmountPaid, @Change, @CashierName, @DateOfTransaction, @Price, @SubTotal, @GrandTotal)";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@BillId", BillId);
                        cmd.Parameters.AddWithValue("@ProductName", item.Name);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@AmountPaid", decimal.Parse(AmountTextBox.Text));
                        cmd.Parameters.AddWithValue("@Change", change);
                        cmd.Parameters.AddWithValue("@CashierName", _user);
                        cmd.Parameters.AddWithValue("@DateOfTransaction", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Price", item.Price);
                        cmd.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                        cmd.Parameters.AddWithValue("@GrandTotal", decimal.Parse(GrandTotalText.Text));

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Bill saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //DeleteAllCartItemsAndTextBoxes();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void DeleteAllCartItemsAndTextBoxes()
        {

            cartModels.Clear();
            CartTable.Items.Refresh();

            AmountTextBox.Clear();
            ChangeText.Text = string.Empty;
            GrandTotalText.Text = "0.00";
            CheckOutBtn.Cursor = Cursors.No;
            CheckOutBtn.IsEnabled = false;

            DisableAmountTextBox();

        }

        private void ReceiptTemplate()
        {

            var checkoutItems = cartModels.ToList();

            Template template = new Template(Convert.ToDecimal(AmountTextBox.Text), change, _user)
            {
                DataContext = checkoutItems

            };
            DeleteAllCartItemsAndTextBoxes();

            template.ShowDialog();

        }
        private void DisableAmountTextBox()
        {
            if (cartModels.Count == 0)
            {
                AmountTextBox.IsEnabled = false;
                AmountTextBox.Clear();
            }
            else
            {
                AmountTextBox.IsEnabled = true;
            }

        }

        private void UpdateStock()
        {
            try
            {
                sqlConnection.Open();

                foreach (var item in cartModels)
                {
                    string query = "UPDATE ProductTbl SET CurrentStock = CurrentStock - @Qty WHERE PrId = @PrId";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                        cmd.Parameters.AddWithValue("@PrId", item.CartId);

                        cmd.ExecuteNonQuery();

                    }
                }

                sqlConnection.Close();
                ProductTable.Items.Refresh();
                DisplayAllProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }

        private void RefundStock()
        {
            try
            {
                sqlConnection.Open();

                foreach (var item in cartModels)
                {
                    string query = "UPDATE ProductTbl SET CurrentStock = CurrentStock + @Qty WHERE PrId = @PrId";

                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                        cmd.Parameters.AddWithValue("@PrId", item.CartId);

                        cmd.ExecuteNonQuery();

                    }
                }

                sqlConnection.Close();
                ProductTable.Items.Refresh();
                DisplayAllProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.AllowOnlyNumbers(sender, e);

        }
    }
}
