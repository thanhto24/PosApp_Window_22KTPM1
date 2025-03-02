using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public ObservableCollection
<Product> ProductList
        { get; } = new()
        {
            new("Hồng Trà Đài Loan", "12,000đ", "ms-appx:///Assets/tea1.jpg"),
            new("Trà Xanh Hoa Nhài", "12,000đ", "ms-appx:///Assets/tea2.jpg"),
            new("Trà Sữa Lài", "20,000đ", "ms-appx:///Assets/tea3.jpg")
        };

        private List
    <string> cartItems = new();
        private double totalAmount = 0;

        public HomeScreen()
        {
            this.InitializeComponent();
            cartListView.ItemsSource = new ObservableCollection
        <string>(cartItems);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Nếu TextBox có nội dung, ẩn placeholder, nếu rỗng thì hiện placeholder
                VisualStateManager.GoToState(textBox, string.IsNullOrEmpty(textBox.Text) ? "Normal" : "TextNotEmpty", true);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Nếu TextBox không có nội dung khi mất focus, quay về trạng thái Normal để hiển thị placeholder
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    VisualStateManager.GoToState(textBox, "Normal", true);
                }
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Parent is StackPanel parent)
            {
                if (parent.Children[1] is TextBlock productName && parent.Children[2] is TextBlock productPrice)
                {
                    string item = $"1x {productName.Text} - {productPrice.Text}";
                    cartItems.Add(item);

                    if (double.TryParse(productPrice.Text.Replace(",", "").Replace("đ", ""), out double price))
                    {
                        totalAmount += price;
                        txtTotal.Text = $"Tổng cộng: {totalAmount:N0}đ";
                    }
                }
            }
        }

        private async void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog saveDialog = new()
            {
                Title = "Lưu đơn hàng",
                Content = "Đơn hàng đã được lưu thành công!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await saveDialog.ShowAsync();
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog checkoutDialog = new()
            {
                Title = "Thanh toán",
                Content = $"Bạn đã thanh toán {totalAmount:N0}đ!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await checkoutDialog.ShowAsync();
            cartItems.Clear();
            totalAmount = 0;
            txtTotal.Text = "Tổng cộng: 0đ";
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        // Constructor mặc định cần thiết để tránh lỗi XAML compiler
        public Product() { }

        // Constructor có tham số để khởi tạo nhanh
        public Product(string name, string price, string imagePath)
        {
            Name = name;
            Price = price;
            ImagePath = imagePath;
        }
    }

}