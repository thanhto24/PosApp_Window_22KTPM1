using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using App.Model;

namespace App.View.Pages
{
    public sealed partial class EditProductDialog : ContentDialog
    {
        public ObservableCollection<Product> Products { get; set; }

        public EditProductDialog()
        {
            this.InitializeComponent();
            Products = new ObservableCollection<Product>(); // Khởi tạo danh sách sản phẩm rỗng
        }

        public EditProductDialog(ObservableCollection<Product> products) : this()
        {
            Products = products;
            CbProductCode.ItemsSource = Products; // Gán danh sách sản phẩm vào ComboBox
        }

        // Sự kiện khi bấm nút "Lưu"
        private void SaveEditProduct(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (CbProductCode.SelectedItem is Product selectedProduct)
            {
                // Cập nhật thông tin sản phẩm
                selectedProduct.Name = TxtProductName.Text;
                selectedProduct.Quantity = int.TryParse(TxtQuantity.Text, out int quantity) ? quantity : selectedProduct.Quantity;
                selectedProduct.Price = int.TryParse(TxtPrice.Text, out int price) ? price : selectedProduct.Price;

                // Cập nhật UI
                var index = Products.IndexOf(selectedProduct);
                if (index != -1)
                {
                    Products[index] = selectedProduct; // Thay đổi trong ObservableCollection để UI cập nhật
                }
            }

            this.Hide();
        }


        // Sự kiện khi bấm nút "Hủy"
        private void CancelEditProduct(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void ProductSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbProductCode.SelectedItem is Product selectedProduct)
            {
                TxtProductName.Text = selectedProduct.Name;
                TxtQuantity.Text = selectedProduct.Quantity.ToString();
                TxtPrice.Text = selectedProduct.Price.ToString();
            }
        }
    }
}
