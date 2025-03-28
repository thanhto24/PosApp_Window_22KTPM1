using App.Model;
using System.Collections.ObjectModel;
using System.Linq;
using App.View.ViewModel; // Import ViewModel
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using App.Service;
using System.Diagnostics;

namespace App.View.Pages
{
    public sealed partial class AllOrdersPage : Page
    {
        public OrderViewModel OrderViewModel { get; set; }
        private bool isAscending = true;
        private string lastSortedColumn = "";
        private int currentPage = -1;
        private int maxPage = -1;

        private int totalOrders;
        private double totalRevenue;
        private double totalProfit;

        public AllOrdersPage()
        {
            this.InitializeComponent();
            OrderViewModel = new OrderViewModel();
            currentPage = 1;
            maxPage = OrderViewModel.TotalPages;
            PageBlock.Text = currentPage.ToString() + " / " + OrderViewModel.TotalPages.ToString();
            setButton();
        }
        private void SortList(object sender, TappedRoutedEventArgs e)
        {
            if (sender is TextBlock header)
            {
                string columnName = header.Tag.ToString();

                if (lastSortedColumn == columnName)
                {
                    isAscending = !isAscending; // Đảo thứ tự nếu nhấn lại cùng cột
                }
                else
                {
                    isAscending = true;
                    lastSortedColumn = columnName;
                }

                // Sắp xếp danh sách theo kiểu phù hợp
                var sortedList = isAscending
                    ? OrderViewModel.DisplayedOrders.OrderBy(x => GetPropertyValue(x, columnName)).ToList()
                    : OrderViewModel.DisplayedOrders.OrderByDescending(x => GetPropertyValue(x, columnName)).ToList();

                // Cập nhật danh sách mà không mất dữ liệu liên kết
                OrderViewModel.DisplayedOrders.Clear();
                foreach (var item in sortedList)
                {
                    OrderViewModel.DisplayedOrders.Add(item);
                }
            }
        }

        private object GetPropertyValue(Order_ order, string propertyName)
        {
            var property = typeof(Order_).GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(order);

                // Nếu giá trị là số, trả về dưới dạng số thay vì chuỗi
                if (value is int || value is double || value is decimal)
                {
                    return Convert.ToDouble(value);
                }

                return value; // Trả về nguyên bản nếu không phải số
            }
            return null;
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage <= 1)
                return;
            OrderViewModel.PreviousPage();
            currentPage = OrderViewModel.CurrentPage;
            PageBlock.Text = currentPage.ToString() + " / " + OrderViewModel.TotalPages.ToString();
            setButton();

        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if(currentPage >= OrderViewModel.TotalPages)
                return;
            OrderViewModel.NextPage();
            currentPage = OrderViewModel.CurrentPage;
            PageBlock.Text = currentPage.ToString() + " / " + OrderViewModel.TotalPages.ToString();
            setButton();

        }

        private void setButton()    
        {
            if (currentPage <= 1)
                PrevOrderBtn.IsEnabled = false;
            else
                PrevOrderBtn.IsEnabled = true;

            if(currentPage >= OrderViewModel.TotalPages)
                NextOrderBtn.IsEnabled = false;
            else
                NextOrderBtn.IsEnabled = true;
        }

        private async void ShowOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Order_ order)
            {
                string orderDetails = string.Join("\n", order.OrderedProducts.Select(p =>
                $"{p.Name}: {p.Quantity} x {p.Price:N0}đ"));


                ContentDialog detailsDialog = new ContentDialog
                {
                    Title = "Chi tiết đơn",
                    Content = new TextBlock { Text = orderDetails, TextWrapping = TextWrapping.Wrap },
                    CloseButtonText = "Đóng",
                    XamlRoot = this.Content.XamlRoot
                };

                await detailsDialog.ShowAsync();
            }
        }
    }
}
