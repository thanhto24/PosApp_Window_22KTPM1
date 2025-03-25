using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using App.Model;
namespace App.View.Pages
{
    public sealed partial class OverviewReport : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }
        public ObservableCollection<Product> Products { get; set; }

        public OverviewReport()
        {
            this.InitializeComponent();
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
            };

            Products = new ObservableCollection<Product>()
            {
                new Product("CF001", "Espresso", 0, 32000, 0, "ms-appx:///Assets/espresso.jpg", "Cà phê", 0.1f, 25000, "8938503270012"),
                new Product("CF002", "Cappuccino", 0, 35000, 0, "ms-appx:///Assets/cappuccino.jpg", "Cà phê", 0.1f, 27000, "8938503270029"),
                new Product("CF003", "Latte", 0, 40000, 0, "ms-appx:///Assets/latte.jpg", "Cà phê", 0.1f, 30000, "8938503270036"),
            };

            this.DataContext = this;
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
    }
}