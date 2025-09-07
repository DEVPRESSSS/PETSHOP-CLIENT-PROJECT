using Microsoft.Data.SqlClient;
using PetShop.DatabaseConnection;
using PetShop.Model;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PetShop.WinForms.UpsertForms.Products
{
    /// <summary>
    /// Interaction logic for UpdateProduct.xaml
    /// </summary>
    public partial class UpdateProduct : Window
    {

        private readonly Connection connection = new Connection();
        private SqlConnection sqlConnection;

        public event EventHandler ProductUpdated;
        public UpdateProduct()
        {

            InitializeComponent();
            sqlConnection = new SqlConnection(connection.ConnectionString);

           
        }
     

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        int productId;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var product = DataContext as ProductModel;

            if(product != null )
            {
                productId= product.ProductId;
                ProductName.Text = product.ProductName;
                Price.Text = product.Price.ToString();
                //Quantity.Text = product.Quantity.ToString();
                ProductName.Text = product.ProductName;
               // Category.SelectedIndex = 1;


            }


        }

        private void Save()
        {
            if(!string.IsNullOrWhiteSpace(ProductName.Text) && !string.IsNullOrWhiteSpace(Price.Text)
                )
            {
                //int qty = Convert.ToInt32(Quantity.Text);

                //if (qty <= 0)
                //{
                //    MessageBox.Show("Qty cant be zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                //    return;
                //}
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("Update ProductTbl set PrName=@PN,PrPrice=@PP,PrCat=@PC where PrId=@PKey", sqlConnection);
                    cmd.Parameters.AddWithValue("@PN", ProductName.Text);
                    cmd.Parameters.AddWithValue("@PC", (ProductCategory.SelectedItem as ComboBoxItem)?.Content.ToString());
                    //cmd.Parameters.AddWithValue("@PQ", Quantity.Text);
                    //cmd.Parameters.AddWithValue("@CurrentStock", Quantity.Text);
                    cmd.Parameters.AddWithValue("@PP", Price.Text);
                    cmd.Parameters.AddWithValue("@PKey", productId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Product Updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);



                    this.Close();

                    ProductUpdated?.Invoke(this, EventArgs.Empty);
                    sqlConnection.Close();

                    DataContext = null;
                    
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

       
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void ProductName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void ProductName_PreviewKeyDown(object sender, KeyEventArgs e)
        {

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
