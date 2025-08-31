using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;
using PetShop.WinForms.UpsertForms.Products;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;


namespace PetShop.UCpages
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;
        private CollectionViewSource collectionViewSource;
        public Products()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);
            collectionViewSource = new CollectionViewSource();

        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var selectedProduct = btn.DataContext as ProductModel;

            if (selectedProduct != null)
            {
                UpdateProduct updateProduct = new UpdateProduct
                {
                    DataContext = selectedProduct
                };

                updateProduct.ProductUpdated += (s, e) => {

                    loadAllProducts();
                };

                updateProduct.ShowDialog();
            }

        }

        private void ProductBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertProcduct insertProcduct = new InsertProcduct();

            insertProcduct.ProductUpdated += (s, e) => {

                loadAllProducts();
            };
            insertProcduct.ShowDialog();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            loadAllProducts();


        }


        private void loadAllProducts()
        {
            sqlConnection.Open();
            string Query = "SELECT PrId, PrName, PrCat, PrQty, PrPrice, CurrentStock, DateCreated FROM ProductTbl";
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
                    DateCreated =(DateTime) reader["DateCreated"],
                    Price = (int)reader["PrPrice"]
                });
            }
            reader.Close();
            sqlConnection.Close();
            collectionViewSource.Source = products;
            ProductTable.ItemsSource = collectionViewSource.View;

        }

     

        private void deleteBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var selectedProduct = btn.DataContext as ProductModel;

            if (selectedProduct != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this product?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                   
                   
                        try
                        {
                            sqlConnection.Open();
                            SqlCommand cmd = new SqlCommand("delete from ProductTbl where PrId = @PKey", sqlConnection);
                            cmd.Parameters.AddWithValue("@PKey", selectedProduct.ProductId);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Product Deleted!!!");
                            sqlConnection.Close();
                            loadAllProducts();
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show(Ex.Message);
                        }
                    
                }
            }
        }

        private void SearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (collectionViewSource.View != null)
            {
                collectionViewSource.View.Filter = item =>
                {
                    var product = item as ProductModel;
                    if (product == null) return false;

                    string searchText = SearchName.Text.ToLower();

                    // Search across ProductModel properties
                    return
                        product.ProductId.ToString().Contains(searchText) ||
                        (!string.IsNullOrEmpty(product.ProductName) && product.ProductName.ToLower().Contains(searchText)) ||
                        (!string.IsNullOrEmpty(product.Category) && product.Category.ToLower().Contains(searchText)) ||
                        product.Quantity.ToString().Contains(searchText) ||
                        product.Price.ToString().Contains(searchText) ||
                        product.CurrentStock.ToString().Contains(searchText) ||
                        product.DateCreated.ToString("yyyy-MM-dd").Contains(searchText);
                };
            }
        }
    }
}
