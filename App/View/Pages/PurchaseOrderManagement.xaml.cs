using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;

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
                new Product("SP001", "Sản phẩm A", 10, 50000, 500000),
                new Product("SP002", "Sản phẩm B", 5, 70000, 350000),
                new Product("SP003", "Sản phẩm C", 20, 30000, 600000)
            };

            this.DataContext = this;
            ProductList.ItemsSource = Products;
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

        public class Product
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public double AverageCost { get; set; }
            public double InventoryValue { get; set; }

            public Product(string code, string name, int quantity, double cost, double value)
            {
                ProductCode = code;
                ProductName = name;
                Quantity = quantity;
                AverageCost = cost;
                InventoryValue = value;
            }
        }
    }
}