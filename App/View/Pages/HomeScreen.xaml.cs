using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Text;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using App.Model;
using App.View.ViewModel;

namespace App.View.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public ProductViewModel ProductModel { get; set; } = new ProductViewModel();
        //public ObservableCollection<Product> ProductList { get; set; } = new ()
        //{
        //    new("Hồng Trà Đài Loan", "12,000đ", "ms-appx:///Assets/tea1.jpg"),
        //    new("Trà Xanh Hoa Nhài", "12,000đ", "ms-appx:///Assets/tea2.jpg"),
        //    new("Trà Sữa Lài", "20,000đ", "ms-appx:///Assets/tea3.jpg")
        //};

        private List<string> cartItems = new();
        private double totalAmount = 0;
        private Button selectedButton = null;

        public HomeScreen()
        {
            this.InitializeComponent();
            cartListView.ItemsSource = new ObservableCollection<string>(cartItems);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                HighlightSelectedButton(clickedButton);
            }
        }

        private void HighlightSelectedButton(Button clickedButton)
        {
            if (selectedButton != null)
            {
                selectedButton.FontWeight = FontWeights.Normal;
                selectedButton.Background = new SolidColorBrush(Colors.White);
                selectedButton.Foreground = new SolidColorBrush(Colors.Black);
            }

            clickedButton.FontWeight = FontWeights.Bold;
            clickedButton.Background = new SolidColorBrush(Colors.DarkOrange);
            clickedButton.Foreground = new SolidColorBrush(Colors.White);

            selectedButton = clickedButton;
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
}
