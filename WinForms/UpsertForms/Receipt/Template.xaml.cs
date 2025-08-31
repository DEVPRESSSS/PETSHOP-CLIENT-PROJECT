using PetShop.Model;
using PetShop.UCpages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PetShop.WinForms.UpsertForms.Receipt
{
    /// <summary>
    /// Interaction logic for Template.xaml
    /// </summary>
    public partial class Template : Window
    {
        decimal _amountPaid = 0;
        decimal _change = 0;
        string _user;
        public Template(decimal amountPaid, decimal change, string user)
        {
            InitializeComponent();
            this._amountPaid = amountPaid;
            this._change = change;
            this._user = user;  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            decimal grandTotal = 0;
            if (DataContext is IEnumerable<CartModel> checkoutItems)
            {
                ItemsList.ItemsSource = checkoutItems;

                foreach(var i in checkoutItems)
                {
                    grandTotal += i.SubTotal;
                }

                TotalText.Text = grandTotal.ToString();
                AmountPaidText.Text = _amountPaid.ToString();
                ChangeText.Text = _change.ToString();

                var date = DateTime.Today.ToString("MM-dd-yyyy");
                DateText.Text = date.ToString();

                string receipt = $"RECEIPT-{Guid.NewGuid().ToString().Substring(0, 6)}";
                ReceiptNoText.Text = receipt;

                var timeNow = DateTime.Now;
                TimeText.Text = TimeOnly.FromDateTime(timeNow).ToString("hh:mm tt");

                CashierText.Text = _user;

            }
            PrintReceipt();
            AutoClose();
        }

        private async void AutoClose()
        {

            await Task.Delay(3000);
            this.Close();
        }

       private void PrintReceipt()
        {

            
                var mainBorder = MainBorder;

                if (mainBorder == null)
                {
                    MessageBox.Show("Could not find the main border control!");
                    return;
                }

                // Create a fixed document for the preview
                FixedDocument document = new FixedDocument();

                // Save current layout settings
                Thickness originalMargin = mainBorder.Margin;
                mainBorder.UpdateLayout();

                // Create a page for the document
                PageContent pageContent = new PageContent();
                FixedPage fixedPage = new FixedPage();

                // Create a container for the entire UI visual
                Canvas container = new Canvas();
                container.Width = mainBorder.ActualWidth;
                container.Height = mainBorder.ActualHeight;

                // Use a visual brush to capture the entire UI
                VisualBrush vb = new VisualBrush(mainBorder);
                Rectangle rect = new Rectangle();
                rect.Width = mainBorder.ActualWidth;
                rect.Height = mainBorder.ActualHeight;
                rect.Fill = vb;
                container.Children.Add(rect);

                // Add the container to the page
                fixedPage.Children.Add(container);
                fixedPage.Width = mainBorder.ActualWidth;
                fixedPage.Height = mainBorder.ActualHeight;
                ((IAddChild)pageContent).AddChild(fixedPage);
                document.Pages.Add(pageContent);

                // Create a window with document viewer
                Window previewWindow = new Window();
                previewWindow.Title = "Print Preview";
                previewWindow.Width = 800;
                previewWindow.Height = 600;
                previewWindow.Owner = Window.GetWindow(this);
                DocumentViewer viewer = new DocumentViewer();
                viewer.Document = document;
                previewWindow.Content = viewer;
                previewWindow.ShowDialog();

                // Restore the original settings
                mainBorder.Margin = originalMargin;
                mainBorder.UpdateLayout();

            
        }
    }
}
