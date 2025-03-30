using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.Model;
using App.View.ViewModel;
using System.Linq;
using App.Utils;
using System.Globalization;
using System.Diagnostics;
using App.Service;
using App.View.Pages;

namespace App.View.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public CategoryViewModel CategoryViewModel { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public CartViewModel CartViewModel { get; set; }
        public OrderViewModel OrderViewModel { get; set; }

        public VoucherViewModel VoucherViewModel { get; set; }
        public CustomerViewModel CustomerViewModel { get; set; }
        private double finalAmount = 0;
        public HomeScreen()
        {
            this.InitializeComponent();
            CategoryViewModel = new CategoryViewModel();
            ProductViewModel = new ProductViewModel();
            CartViewModel = new CartViewModel();
            OrderViewModel = new OrderViewModel();

            VoucherViewModel = new VoucherViewModel();
            CustomerViewModel = new CustomerViewModel();

            if(CategoryViewModel.categories.Any() && ProductViewModel.Products.Any())
                ProductViewModel.LoadProductsByCategory(CategoryViewModel.categories[0].Name);

            ApplyDiscount();
        }

        private void DrinkCategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrinkCategoryList.SelectedItem is Category_ selectedCategory)
            {
                string selectedTypeGroup = selectedCategory.Name;
                ProductViewModel.LoadProductsByCategory(selectedTypeGroup);
            }
        }


        // Xử lý sự kiện khi bấm nút "+"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                product.Quantity++;
                CartViewModel.AddToCart(product);
                OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
                ApplyDiscount();
            }
        }

        // Xử lý sự kiện khi bấm nút "-"
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                product.Quantity--;
                CartViewModel.RemoveFromCart(product);
                OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
                ApplyDiscount();
            }
        }


        private void PromoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyDiscount();
        }

        private void CustomerCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyDiscount();
        }

        private void ApplyDiscount()
        {
            double total = CartViewModel.getTotalAmount();
            double discountVc = 0, discountCustomer = 0;

            string promoCode = PromoCodeTextBox.Text.Trim();
            string phone = CustomerCodeTextBox.Text.Trim();

            // Áp dụng mã khuyến mãi
            discountVc = total * VoucherViewModel.ApplyVoucher(promoCode);
            discountCustomer = total * CustomerViewModel.ApplyCusPhone(phone);

            finalAmount = total - discountVc - discountCustomer;
            CartViewModel.totalDiscount = discountVc + discountCustomer;

            // Hiển thị số tiền trên giao diện
            TotalAmountTextBlock.Text = $"{total:N0}đ";
            DiscountAmountTextBlock.Text = $"-{discountVc + discountCustomer:N0}đ";
            FinalAmountTextBlock.Text = $"{finalAmount:N0}đ";
        }

        private void ClearCart_Click(object sender, RoutedEventArgs e)
        {
            CartViewModel.Clear_();
            PromoCodeTextBox.Text = "";
            CustomerCodeTextBox.Text = "";
            ApplyDiscount();
            OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ApplyDiscount();
            string customerName = string.IsNullOrWhiteSpace(CustomerName.Text) ? "Khách vãng lai" : CustomerName.Text;
            CartViewModel.CreateNewOrder(CartViewModel.totalDiscount, customerName);


            string phone = CustomerCodeTextBox.Text.Trim();
            string name = CustomerName.Text.Trim();
            CustomerViewModel.storeData(phone, name, finalAmount);

            string promoCode = PromoCodeTextBox.Text.Trim();
            VoucherViewModel.des(promoCode);

            ContentDialog checkoutDialog = new ContentDialog
            {
                Title = "Thông báo",
                Content = $"Tổng thanh toán: {FinalAmountTextBlock.Text}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };



            await checkoutDialog.ShowAsync();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Debug.WriteLine($"New Height: {e.NewSize.Height}");

            if (e.NewSize.Height < 450)
            {
                OrderScrollViewer.MaxHeight = 30;
                TongTien.Margin = new Thickness(0, 5, 0, 0);
                TienGiam.Margin = new Thickness(0, 5, 0, 0);
                TienTra.Margin = new Thickness(0, 5, 0, 0);

            }
            else
            {
                OrderScrollViewer.MaxHeight = 180;
                TongTien.Margin = new Thickness(0, 25, 0, 0);
                TienGiam.Margin = new Thickness(0, 25, 0, 0);
                TienTra.Margin = new Thickness(0, 25, 0, 0);
            }

            //Debug.WriteLine($"OrderScrollViewer MaxHeight: {OrderScrollViewer.MaxHeight}");
        }


    }
}