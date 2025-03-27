using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using App.Model;
using App.Service;
using System.Linq;

namespace App.View.Pages
{
    public sealed partial class OverviewReport : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<ReportData> Reports { get; set; } = new ObservableCollection<ReportData>();
        private readonly MockDao mockDao = new MockDao();

        public OverviewReport()
        {
            this.InitializeComponent();
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
            };

            this.DataContext = this;
            LoadReportData();
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

        private void LoadReportData()
        {
            CalculateSummary(); // Gọi tính toán và chèn report mới

            // Lấy danh sách cập nhật sau khi insert
            var reports = mockDao.Reports.GetAll();

            // Xóa dữ liệu cũ và cập nhật mới
            Reports.Clear();
            foreach (var report in reports)
            {
                Reports.Add(report);
                Debug.WriteLine($"[DEBUG] Report: Orders = {report.TotalOrders}, Revenue = {report.TotalRevenue}, Profit = {report.TotalProfit}");
            }

            UpdateReportUI();
        }

        private void UpdateReportUI()
        {
            if (Reports.Count > 0)
            {
                var latestReport = Reports[^1]; // Lấy báo cáo mới nhất
                OrdersCountText.Text = latestReport.TotalOrders.ToString();
                RevenueText.Text = $"{latestReport.TotalRevenue:N0}đ";
                ProfitText.Text = $"{latestReport.TotalProfit:N0}đ";
            }
        }

        private void CalculateSummary()
        {
            var orders = mockDao.Orders.GetAll();

            int totalOrders = orders.Count;
            decimal totalRevenue = orders.Sum(o => o.TotalAmount); // Tổng tiền của tất cả đơn hàng
            decimal totalCost = orders.Sum(o => o.TotalCost); // Tổng chiết khấu của tất cả đơn hàng
            decimal totalProfit = totalRevenue - totalCost; // Tổng lợi nhuận

            // Debug để kiểm tra từng giá trị
            foreach (var o in orders)
            {
                Debug.WriteLine($"[DEBUG] Order: TotalAmount = {o.TotalAmount}, TotalDiscount = {o.TotalDiscount}");
            }

            Debug.WriteLine($"[DEBUG] Summary: Orders = {totalOrders}, Revenue = {totalRevenue}, TotalDiscount = {totalCost}, Profit = {totalProfit}");

            var report = new ReportData(totalOrders, totalRevenue, totalProfit);
            mockDao.Reports.Insert(report);
        }



    }
}