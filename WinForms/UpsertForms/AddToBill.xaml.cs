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

namespace PetShop.WinForms.UpsertForms
{
    /// <summary>
    /// Interaction logic for AddToBill.xaml
    /// </summary>
    public partial class AddToBill : Window
    {

        public event EventHandler AddedToCartEvent;
        //public event EventHandler CloseTheForm;

        public AddToBill()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //CloseTheForm?.Invoke(this, EventArgs.Empty);
            this.Close();

        }



        private void AddToBillBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }


        int productId = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var product = DataContext as ProductModel;

            if (product != null)
            {
                productId = product.ProductId;
                ProductName.Text = product.ProductName;
                Price.Text = product.Price.ToString();
                Stock.Text = product.CurrentStock.ToString();

            }
            AddToBillBtn.IsEnabled = false;

        }

        private void AddToBillBtn_Click_1(object sender, RoutedEventArgs e)
        {


            CartModel obj = new CartModel()
            {
                CartId = productId, 
                Name = ProductName.Text,
                Price = Convert.ToDecimal(Price.Text),
                Quantity = Convert.ToInt32(Qty.Text),
                SubTotal = Convert.ToDecimal(Price.Text) * Convert.ToInt32(Qty.Text),
                
            };

           
            AddedToCartEvent?.Invoke(obj, EventArgs.Empty);

            this.Close();
        }

        int qty =0;

        private void Qty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(Qty.Text, out qty) && qty > 0)
            {
                AddToBillBtn.IsEnabled = true;

                if (qty > Convert.ToInt32( Stock.Text)) {

                    MessageBox.Show("The quantity you entered is greater than the current stock","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Qty.Text = "";
                    return;
                }


            }
            else
            {
                AddToBillBtn.IsEnabled = false;
            }
        }

        private void Qty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            HelperValidation.InputValidator.AllowOnlyNumbers(sender, e);

        }

        private void Qty_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HelperValidation.InputValidator.Username(sender, e);

        }
    }
}
