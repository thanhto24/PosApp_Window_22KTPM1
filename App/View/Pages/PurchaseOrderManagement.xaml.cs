using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace App.View.Pages
{
    public sealed partial class PurchaseOrderManagement : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }

        public PurchaseOrderManagement()
        {
            this.InitializeComponent();
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
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
