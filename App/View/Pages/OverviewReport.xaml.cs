using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using App.Model;
using App.Service;
using System.Linq;
using App.View.ViewModel;

namespace App.View.Pages
{
    public sealed partial class OverviewReport : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }
        //public ObservableCollection<Product> Products { get; set; }
        public ReportViewModel ReportViewModel { get; set; }
        public OrderViewModel OrderViewModel { get; set; }

        //cho biểu đồ:
        public LineChartViewModel LineChartViewModel { get; set; }



        public decimal totalRevenue;
        public decimal totalCost;
        public decimal totalProfit;
        public int totalOrders;



        public OverviewReport()
        {
            this.InitializeComponent();
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
            };
            OrderViewModel = new OrderViewModel();
            ReportViewModel = new ReportViewModel();
            LineChartViewModel = new LineChartViewModel();

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
            // Cập nhật dữ liệu sau khi thay đổi ngày
            LoadReportData();
        }

        // Áp dụng ngày tùy chỉnh
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateText.Text = $"{StartDatePicker.Date:dd/MM/yyyy} - {EndDatePicker.Date:dd/MM/yyyy}";

            if (DatePickerFlyout.IsOpen)
            {
                DatePickerFlyout.Hide();
            }
            // Cập nhật dữ liệu sau khi thay đổi ngày
            LoadReportData();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ChartViewMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportViewModel == null || LineChartViewModel == null)
                return;

            string timeFrame = "day";

            if (ChartViewMode.SelectedIndex == 1)
                timeFrame = "month";
            else if (ChartViewMode.SelectedIndex == 2)
                timeFrame = "year";

            // Cập nhật biểu đồ với chế độ đã chọn
            LineChartViewModel.UpdateChart(ReportViewModel.GetAllOrders(), timeFrame);
        }

        private void LoadReportData()
        {
            CalculateSummary();

            // Xóa dữ liệu cũ và cập nhật mới
            //ReportViewModel.report.Clear();
            //foreach (var report in ReportViewModel.report)
            //{
            //    ReportViewModel.report.Add(report);
            //    Debug.WriteLine($"[DEBUG] Report: Orders = {report.TotalOrders}, Revenue = {report.TotalRevenue}, Profit = {report.TotalProfit}");
            //}

            UpdateReportUI();

            // Cập nhật biểu đồ với chế độ xem hiện tại
            string timeFrame = "day";
            if (ChartViewMode != null)
            {
                if (ChartViewMode.SelectedIndex == 1)
                    timeFrame = "month";
                else if (ChartViewMode.SelectedIndex == 2)
                    timeFrame = "year";
            }
            LineChartViewModel.UpdateChart(ReportViewModel.GetAllOrders(), timeFrame);
        }

        private void UpdateReportUI()
        {
            //var latestReport = ReportViewModel.report[^1]; // Lấy báo cáo mới nhất
            Debug.WriteLine($"[DEBUG] Report Count: {ReportViewModel.report.Count}");
            Debug.WriteLine($"[DEBUG] Reportkkkkk: Revenue = {totalRevenue}, Profit = {totalProfit}, TotalDiscount = {totalCost}");

            OrdersCountText.Text = totalOrders.ToString();
            RevenueText.Text = $"{totalRevenue:N0}đ";
            ProfitText.Text = $"{totalProfit:N0}đ";
        }

        private void CalculateSummary()
        {
            var orders = ReportViewModel.GetAllOrders();
            Debug.WriteLine($"[DEBUG] Orders Count: {orders.Count}");
            totalOrders = orders.Count;
            totalRevenue = orders.Sum(o => o.TotalAmount); // Tổng tiền của tất cả đơn hàng
            totalCost = orders.Sum(o => o.TotalCost); // Tổng chiết khấu của tất cả đơn hàng
            totalProfit = totalRevenue - totalCost; // Tổng lợi nhuận

            foreach (var o in orders)
            {
                Debug.WriteLine($"[DEBUG] Order: TotalAmount = {o.TotalAmount}, TotalDiscount = {o.TotalDiscount}");
            }

            Debug.WriteLine($"[DEBUG] Summary: Orders = {totalOrders}, Revenue = {totalRevenue}, TotalDiscount = {totalCost}, Profit = {totalProfit}");
            //var report = new ReportData(totalOrders, totalRevenue, totalProfit);
            //ReportViewModel.addReport(report);

        }



    }
}