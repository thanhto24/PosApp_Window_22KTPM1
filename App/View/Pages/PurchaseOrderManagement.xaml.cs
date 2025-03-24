using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using App.Model;
using System.Linq;

namespace App.View.Pages
{
    public sealed partial class PurchaseOrderManagement : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }
        public ObservableCollection<Product> Products { get; set; }

        public PurchaseOrderManagement()
        {
            this.InitializeComponent();
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
            };

            Products = new ObservableCollection<Product>()
            {
                new Product("SP001", "Sản phẩm A", 10, 10000, 100000, "Images/productA.jpg", "Loại A", 0.1f, 8000, "8938503270012"),
                new Product("SP002", "Sản phẩm B", 5, 140000, 700000, "Images/productB.jpg", "Loại B", 0.1f, 120000, "8938503270029"),
                new Product("SP003", "Sản phẩm C", 20, 60000, 1200000, "Images/productC.jpg", "Loại C", 0.1f, 50000, "8938503270036")
            };




            this.DataContext = this;
            ProductList.ItemsSource = Products;

            // Cập nhật tổng giá trị tồn kho và tổng số lượng
            UpdateTotals();

            // Đảm bảo tổng sẽ tự cập nhật khi danh sách thay đổi
            Products.CollectionChanged += (s, e) => UpdateTotals();
        }

        // Hiển thị Flyout khi bấm vào nút chọn ngày
        private void DatePickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Flyout is Flyout flyout)
            {
                flyout.ShowAt(btn);
            }
        }

        // Xử lý chọn preset ngày nhanh
        private void PresetDate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                SelectedDateText.Text = btn.Content.ToString();

                // Kiểm tra nếu Flyout mở thì mới đóng
                if (DatePickerFlyout.IsOpen)
                {
                    DatePickerFlyout.Hide();
                }
            }
        }

        // Áp dụng ngày tùy chỉnh
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateText.Text = $"{StartDatePicker.Date:dd/MM/yyyy} - {EndDatePicker.Date:dd/MM/yyyy}";

            if (DatePickerFlyout.IsOpen)
            {
                DatePickerFlyout.Hide();
            }
        }

        // Hủy chọn ngày
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerFlyout.IsOpen)
            {
                DatePickerFlyout.Hide();
            }
        }

        private void TxtCategory_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            CbCategory.IsDropDownOpen = true; // Open ComboBox dropdown
        }

        private void TxtCategoryWare_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            CbCategoryWare.IsDropDownOpen = true; // Open ComboBox dropdown
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ResetInventory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditProductDialog(Products); // Truyền danh sách sản phẩm

            // Gán XamlRoot nếu cần thiết để tránh lỗi
            if (this.XamlRoot != null)
            {
                dialog.XamlRoot = this.XamlRoot;
            }

            await dialog.ShowAsync();
        }

        private void UpdateTotals()
        {
            int totalQuantity = Products.Sum(p => p.Quantity);
            decimal totalValue = Products.Sum(p => p.TotalPrice); // Tổng giá trị tồn kho

            TxtTotalQuantity.Text = totalQuantity.ToString();
            TxtTotalValue.Text = $"{totalValue:N0} đ"; // Định dạng số có dấu chấm phân cách
        }
    }
}