using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.View.Dialogs;
using App.View.ViewModel;
using App.Model;
using App.Utils;
using System.Collections.Generic;

namespace App.View.Pages
{
    public sealed partial class ProductPage : Page
    {
        public ProductViewModel ProductModelPage { get; set; }

        public ProductPage()
        {
            this.InitializeComponent();

            try
            {
                //// Get the Window instance and pass it to ViewModel
                //var window = WindowHelper.GetWindowForElement(this);
                ProductModelPage = new ProductViewModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing ProductPage: {ex.Message}");
                ProductModelPage = new ProductViewModel(); // Fallback without window
            }
        }


        // Add New Product Button Click
        private async void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            await ShowProductDialog();
        }

        // Edit Product Button Click
        private async void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //string barCode = button?.CommandParameter as string;

            //if (!string.IsNullOrEmpty(barCode))
            //{
            //    // Find the product with matching barCode
            //    Product product = ProductModelPage.Products.Find(p => p.BarCode == barCode);
            //    if (product != null)
            //    {
            //        await ShowProductDialog(product);
            //    }
            //}
        }

        // Delete Product Button Click
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // Get parent StackPanel
            if (button?.Parent is StackPanel stackPanel &&
                stackPanel.Parent is StackPanel productPanel &&
                productPanel.DataContext is Product product)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Xác nhận xóa",
                    Content = $"Bạn có chắc chắn muốn xóa sản phẩm '{product.Name}'?",
                    PrimaryButtonText = "Xóa",
                    CloseButtonText = "Hủy",
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    bool deleted = await ProductModelPage.DeleteProduct(product);
                    if (!deleted)
                    {
                        await ShowErrorDialog("Không thể xóa sản phẩm. Vui lòng thử lại sau.");
                    }
                }
            }
        }

        // Show Product Dialog for Add/Edit
        private async Task ShowProductDialog(Product productToEdit = null)
        {
            try
            {
                var dialogContent = new ProductDialogContent(ProductModelPage, productToEdit);

                ContentDialog dialog = new ContentDialog
                {
                    Title = productToEdit == null ? "Thêm sản phẩm mới" : "Chỉnh sửa sản phẩm",
                    Content = dialogContent,
                    PrimaryButtonText = "Lưu",
                    SecondaryButtonText = "Hủy",
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Product product = await dialogContent.GetProductAsync();
                    StorageFile imageFile = dialogContent.GetSelectedImageFile();

                    if (product != null)
                    {
                        bool success;
                        if (productToEdit == null)
                        {
                            // Add new product
                            Debug.WriteLine("CHECK PRODUCT BEFORE ADD: " + product.Name + " " + imageFile.ToString());
                            if (imageFile == null) Debug.WriteLine("CHECK IMAGE FILE IS NULL: ", imageFile);
                            success = await ProductModelPage.AddProduct(product, imageFile);
                        }
                        else
                        {
                            // Update existing product
                            success = await ProductModelPage.UpdateProduct(product, imageFile);
                        }

                        if (!success)
                        {
                            await ShowErrorDialog("Không thể lưu sản phẩm. Vui lòng thử lại sau.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog($"Lỗi: {ex.Message}");
            }
        }

        private async Task ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductModelPage != null)
               ApplyFilters();
        }

        // Apply all filters
        private async void ApplyFilters()
        {
            return;

            string searchText = string.Empty;
            if (SearchTextBox != null && !string.IsNullOrEmpty(SearchTextBox.Text))
            {
                searchText = SearchTextBox.Text;
            }

            string productType = "Tất cả";
            if (ProductTypeComboBox != null && (ProductTypeComboBox.SelectedItem is ComboBoxItem typeItem))
            {
                productType = typeItem.Content.ToString();
            }

            string productGroup = "Tất cả";
            if (ProductGroupComboBox != null && (ProductGroupComboBox.SelectedItem is ComboBoxItem groupItem))
            {
                productGroup = groupItem.Content.ToString();
            }

            string status = "Tất cả";
            if (StatusComboBox != null && (StatusComboBox.SelectedItem is ComboBoxItem statusItem))
            {
                status = statusItem.Content.ToString();
            }

            string sortOrder = "Tên: A => Z";
            if (SortOrderComboBox != null && (SortOrderComboBox.SelectedItem is ComboBoxItem sortItem))
            {
                sortOrder = sortItem.Content.ToString();
            }

            var filter = new Dictionary<string, object>();

            // Use the new filter method from ViewModel
            //await ProductModelPage.FilterProducts(searchText, productType, productGroup, status, sortOrder);

            //var filter = new Dictionary<string, object>
            //{
            //    {"Name", searchText + "%" }
            //};

            if (searchText != "")
                filter.Add("Name", searchText + "%");

            // Add TypeGroup filter if not "Tất cả"
            if (productType != "Tất cả")
            {
                filter.Add("TypeGroup", productType);
            }

            // Add product group filter if not "Tất cả"
            if (productGroup != "Tất cả")
            {
                filter.Add("Group", productGroup);
            }

            // Add status filter if not "Tất cả"
            if (status != "Tất cả")
            {
                bool isActive = status == "Còn hàng";
                filter.Add("IsActive", isActive);
            }

            // Set up sort order
            Dictionary<string, int> sort = new Dictionary<string, int>();

            switch (sortOrder)
            {
                case "Tên: A => Z":
                    sort.Add("Name", 1);
                    break;
                case "Tên: Z => A":
                    sort.Add("Name", -1);
                    break;
                case "Giá: Thấp => Cao":
                    sort.Add("Price", 1);
                    break;
                case "Giá: Cao => Thấp":
                    sort.Add("Price", -1);
                    break;
                case "Ngày cập nhật: Cũ nhất":
                    sort.Add("LastUpdated", 1);
                    break;
                case "Ngày cập nhật: Mới nhất":
                    sort.Add("LastUpdated", -1);
                    break;
                default:
                    sort.Add("Name", 1); // Default sort
                    break;
            }

            var sortQuery = sort;
            if (ProductModelPage == null)
            {
                //ProductModelPage = new ProductViewModel();
                Debug.WriteLine("ProductPage", "ProductModelPage is null");
            }
            await ProductModelPage.NewFilter(filter, sort);
        }

        private void SearchTextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Check if Enter key was pressed
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ApplyFilters();
            }
        }
    }
}