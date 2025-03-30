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
using Microsoft.UI.Xaml.Media;

namespace App.View.Pages
{
    public sealed partial class ProductPage : Page
    {
        public ProductViewModel ProductModelPage { get; set; }

        public ProductPage()
        {
            this.InitializeComponent();
            ProductModelPage = new ProductViewModel();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        // Apply all filters
        private void ApplyFilters()
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

            // Add search text filter if not empty
            if (!string.IsNullOrEmpty(searchText))
            {
                filter.Add("Name", searchText + "%");
            }

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

            // Apply the filters and sorting
            ProductModelPage.NewFilter(filter, sort);
        }

        private void SearchTextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Check if Enter key was pressed
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ApplyFilters();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            double fontSize = 16;
            double buttonSize = 40;

            if (width < 670)
                fontSize = 8;
            else if (width < 700)
                fontSize = 10;
            else if (width < 800)
                fontSize = 11;
            else if (width < 1000)
                fontSize = 12;
            else if (width < 1100)
                fontSize = 14;
            else
                fontSize = 16;

            Debug.WriteLine($"Current Width: {width}, Font Size: {fontSize}, Button Size: {buttonSize}");

            UpdateFontSize(GridRoot, fontSize);
        }



        private void UpdateFontSize(DependencyObject parent, double fontSize)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is TextBlock textBlock)
                {
                    textBlock.FontSize = fontSize;
                }

                UpdateFontSize(child, fontSize);
            }
        }
    }
}