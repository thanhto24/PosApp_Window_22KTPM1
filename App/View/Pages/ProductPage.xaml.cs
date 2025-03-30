﻿using System;
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
            var button = sender as Button;
            if (button?.DataContext is Product product)
            {
                await ShowProductDialog(product);
            }
        }

        // Delete Product Button Click
        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Product product)
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

        private async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductModelPage != null)
                await ApplyFilters();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplyFilters();
        }

        // Apply all filters
        private async Task ApplyFilters()
        {
            string searchText;
            if (SearchTextBox != null && !string.IsNullOrEmpty(SearchTextBox.Text))
            {
                searchText = SearchTextBox.Text;
            }
            else
            {
                searchText = "";
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

            //Debug.WriteLine("ProductPage", $"Filters: {searchText}, {productType}, {productGroup}, {status}, {sortOrder}");

            // Use the new filter method from ViewModel
            //await ProductModelPage.FilterProducts(searchText, productType, productGroup, status, sortOrder);

            //var filter = new Dictionary<string, object>
            //{
            //    {"Name", searchText + "%" }
            //};
            var filter = new Dictionary<string, object>();

            if (searchText != "")
                filter.Add("Name", searchText + "%");

            // Chỉ thêm TypeGroup nếu productType không phải là "Tất cả"
            if (productType != "Tất cả")
                filter.Add("TypeGroup", productType);


            // Điều kiện OR
            //var or = new Dictionary<string, object>
            //{
            //    { "name", "Alice" },
            //    { "city", "New York" }
            //};
            // Điều kiện sắp xếp
            Dictionary<string, int> sort = new Dictionary<string, int> { { "Name", 1 } }; // Mặc định sắp xếp theo tên A-Z

            switch (sortOrder)
            {
                case "Tên: A => Z":
                    sort = new Dictionary<string, int> { { "Name", 1 } };
                    break;
                case "Tên: Z => A":
                    sort = new Dictionary<string, int> { { "Name", -1 } };
                    break;
                case "Giá: Thấp => Cao":
                    sort = new Dictionary<string, int> { { "Price", 1 } };
                    break;
                case "Giá: Cao => Thấp":
                    sort = new Dictionary<string, int> { { "Price", -1 } };
                    break;
                case "Ngày cập nhật: Cũ nhất":
                    sort = new Dictionary<string, int> { { "LastUpdated", 1 } };
                    break;
                case "Ngày cập nhật: Mới nhất":
                    sort = new Dictionary<string, int> { { "LastUpdated", -1 } };
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
        private async void SearchTextBox_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await ApplyFilters();
            }
        }


    }
}