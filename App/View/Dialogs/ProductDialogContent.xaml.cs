using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using App.Model;
using App.View.Pages;
using App.View.ViewModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.View.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductDialogContent : UserControl
    {
        private StorageFile _selectedImageFile;
        private Product _editingProduct;
        private bool _isEditMode;
        private readonly ProductViewModel _viewModel;

        public ProductDialogContent(ProductViewModel viewModel, Product product = null)
        {
            this.InitializeComponent();
            _viewModel = viewModel;

            if (product != null)
            {
                _editingProduct = product;
                _isEditMode = true;
                LoadProductData();
            }
            else
            {
                _editingProduct = new Product();
                _isEditMode = false;

                // Generate a unique barcode for a new product
                _editingProduct.BarCode = GenerateUniqueBarcode();
                TxtBarcode.Text = _editingProduct.BarCode;
            }
        }

        private void LoadProductData()
        {
            TxtName.Text = _editingProduct.Name;
            TxtPrice.Text = _editingProduct.Price.ToString();
            TxtBarcode.Text = _editingProduct.BarCode;
            ComboTypeGroup.SelectedItem = _editingProduct.TypeGroup;
            TxtVAT.Text = _editingProduct.Vat.ToString();
            TxtCostPrice.Text = _editingProduct.CostPrice.ToString();

            // Load image if available
            if (!string.IsNullOrEmpty(_editingProduct.ImagePath))
            {
                LoadImage(_editingProduct.ImagePath);
            }
        }

        private async void LoadImage(string imagePath)
        {
            // Kiểm tra xem imagePath có bắt đầu với "ms-appx://" không
            if (imagePath.StartsWith("ms-appx:///"))
            {
                // Loại bỏ "ms-appx://" khỏi chuỗi
                imagePath = imagePath.Substring("ms-appx:///".Length);
            }
            try
            {
                BitmapImage bitmapImage = await ProductViewModel.LoadImageFromPath(imagePath);
                if (bitmapImage != null)
                {
                    ProductImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private async void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedImageFile = await _viewModel.PickProductImage();

                if (_selectedImageFile != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (var stream = await _selectedImageFile.OpenReadAsync())
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }
                    ProductImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error selecting image: " + ex.Message);
                //var dialog = new ContentDialog
                //{
                //    Title = "Error",
                //    Content = $"Failed to select image: {ex.Message}",
                //    CloseButtonText = "OK"
                //};
                //await dialog.ShowAsync();
            }
        }

        private string GenerateUniqueBarcode()
        {
            // Simple implementation - in production, you should use a more robust algorithm
            return $"PRD{DateTime.Now.ToString("yyyyMMddHHmmss")}";
        }

        public async Task<Product> GetProductAsync()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtPrice.Text) ||
                ComboTypeGroup.SelectedItem == null)
            {
                return null;
            }

            // Update product properties
            _editingProduct.Name = TxtName.Text;
            _editingProduct.Price = int.Parse(TxtPrice.Text);
            _editingProduct.BarCode = TxtBarcode.Text;
            _editingProduct.TypeGroup = (ComboTypeGroup.SelectedItem as ComboBoxItem).Content.ToString();

            System.Diagnostics.Debug.WriteLine($"check typeGroup: {_editingProduct.TypeGroup}");

            // Parse VAT value
            if (float.TryParse(TxtVAT.Text, out float vat))
            {
                _editingProduct.Vat = vat;
            }
            else
            {
                _editingProduct.Vat = 0;
            }

            _editingProduct.CostPrice = int.Parse(TxtCostPrice.Text);

            return _editingProduct;
        }

        public StorageFile GetSelectedImageFile()
        {
            return _selectedImageFile;
        }
    }
}
