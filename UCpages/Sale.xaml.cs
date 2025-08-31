using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
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

namespace PetShop.UCpages
{
    /// <summary>
    /// Interaction logic for Sale.xaml
    /// </summary>
    public partial class Sale : UserControl
    {
        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private CollectionViewSource collectionViewSource; 

        public Sale()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            collectionViewSource = new CollectionViewSource();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllSales();
        }

        private void LoadAllSales()
        {
            sqlConnection.Open();
            string query = @"
                        SELECT
                            BillId,
                            ProductName,
                            Quantity,
                            AmountPaid,
                            Change,
                            CashierName,
                            DateOfTransaction,
                            Price,
                            SubTotal,
                            GrandTotal
                        FROM dbo.[Transaction];";
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = cmd.ExecuteReader();

            List<SalesModel> sales = new List<SalesModel>();
            while (reader.Read())
            {
                sales.Add(new SalesModel
                {
                    BillId = reader["BillId"].ToString(),
                    ProductName = reader["ProductName"].ToString(),
                    Quantity = (int)reader["Quantity"],
                    AmountPaid = (decimal)reader["AmountPaid"],
                    Price = (decimal)reader["Price"],
                    Change = (decimal)reader["Change"],
                    SubTotal = (decimal)reader["SubTotal"],
                    GrandTotal = (decimal)reader["GrandTotal"],
                    CashierName =reader["CashierName"].ToString(),
                    DateOfTransaction = DateOnly.FromDateTime(
                    Convert.ToDateTime(reader["DateOfTransaction"]))
                });
            }
            reader.Close();
            sqlConnection.Close();
            // Bind to CollectionViewSource
            collectionViewSource.Source = sales;
            SaleTable.ItemsSource = collectionViewSource.View;

        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (collectionViewSource.View != null)
            {
                collectionViewSource.View.Filter = item =>
                {
                    var sale = item as SalesModel;
                    if (sale == null) return false;

                    string searchText = SearchName.Text.ToLower();

                    // Search across ALL properties
                    return
                        (sale.BillId != null && sale.BillId.ToLower().Contains(searchText)) ||
                        (sale.ProductName != null && sale.ProductName.ToLower().Contains(searchText)) ||
                        (sale.CashierName != null && sale.CashierName.ToLower().Contains(searchText)) ||
                        sale.Quantity.ToString().Contains(searchText) ||
                        sale.AmountPaid.ToString().Contains(searchText) ||
                        sale.Price.ToString().Contains(searchText) ||
                        sale.Change.ToString().Contains(searchText) ||
                        sale.SubTotal.ToString().Contains(searchText) ||
                        sale.GrandTotal.ToString().Contains(searchText) ||
                        sale.DateOfTransaction.ToString("yyyy-MM-dd").Contains(searchText);
                };
            }
        }
    }
}
