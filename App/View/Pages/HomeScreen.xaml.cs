using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.Model;
using System.Linq;
using System.Collections.Generic;
using App.Service;
using App.View.ViewModel;
using System;

namespace App.View.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> OrderedProducts { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Category_> Categories { get; set; } = new ObservableCollection<Category_>();

        private readonly MockDao mockDao = new MockDao();
        private Dictionary<string, List<Product>> categoryProducts = new();
        public CategoryViewModel CategoryViewModel { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public CartViewModel CartViewModel { get; set; }

        private string currentCategory = "Cà phê";

        public HomeScreen()
        {
            this.InitializeComponent();
            this.DataContext = this;

            CategoryViewModel = new CategoryViewModel();
            ProductViewModel = new ProductViewModel();
            CartViewModel = new CartViewModel();
            ApplyDiscount();

            foreach (var category in mockDao.Categories.GetAll())
            {
                Categories.Add(category);
            }

            LoadProductsByCategory(currentCategory);
        }

        private void DrinkCategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrinkCategoryList.SelectedItem is Category_ selectedCategory)
            {
                string selectedTypeGroup = selectedCategory.Name;
                Debug.WriteLine($"Chọn danh mục: {selectedTypeGroup}");

                SaveProductQuantities(currentCategory);
                currentCategory = selectedTypeGroup;
                RestoreProductQuantities(selectedTypeGroup);
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                product.Quantity++;
                UpdateOrderSummary();
                CartViewModel.AddToCart(product);
                UpdateOrderList(product);

            }
        }


        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                if (product.Quantity > 0)
                {
                    product.Quantity--;
                    CartViewModel.RemoveFromCart(product);
                    UpdateOrderSummary();
                    UpdateOrderList(product);
                }
            }
        }


        private void UpdateOrderList(Product product)
        {
            var existingProduct = OrderedProducts.FirstOrDefault(p => p.ProductCode == product.ProductCode);

            if (product.Quantity > 0)
            {
                if (existingProduct == null)
                {
                    OrderedProducts.Add(product);
                }
                else
                {
                    existingProduct.Quantity = product.Quantity;
                }
            }
            else
            {
                if (existingProduct != null)
                {
                    OrderedProducts.Remove(existingProduct);
                }
            }
            UpdateOrderSummary();
        }

        private void UpdateOrderSummary()
        {
            int totalItems = OrderedProducts.Sum(p => p.Quantity);
            OrderSummaryText.Text = $"Các món order ({totalItems} món)";
            ApplyDiscount();
        }

        private void SaveProductQuantities(string category)
        {
            if (!categoryProducts.ContainsKey(category))
            {
                categoryProducts[category] = new List<Product>();
            }

            categoryProducts[category] = Products.Select(p =>
                new Product(p.ProductCode, p.Name, p.Quantity, p.Price, p.TotalPrice, p.ImagePath, p.TypeGroup, p.VAT, p.CostPrice, p.BarCode)).ToList();
        }

        private void RestoreProductQuantities(string category)
        {
            Products.Clear();
            if (categoryProducts.TryGetValue(category, out var savedProducts))
            {
                foreach (var product in savedProducts)
                {
                    Products.Add(product);
                }
            }
            else
            {
                LoadProductsByCategory(category);
            }
        }

        private void LoadProductsByCategory(string typeGroup)
        {
            Products.Clear();

            if (mockDao.Products is MockDao.MockProductRepository productRepo)
            {
                var filteredProducts = productRepo.GetProductsByCategory(typeGroup);
                foreach (var product in filteredProducts)
                {
                    Products.Add(product);
                }
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

            // Áp dụng mã khuyến mãi
            if (promoCode == "GIAM50") discountVc = total * 0.5;  // Giảm 50%
            else if (promoCode == "GIAM10") discountVc = total * 0.1;  // Giảm 10%
            string sdt = CustomerCodeTextBox.Text.Trim();
            if (sdt == "999") discountCustomer = total * 0.1;

            double finalAmount = total - discountVc - discountCustomer;

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
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ApplyDiscount();

            ContentDialog checkoutDialog = new ContentDialog
            {
                Title = "Thông báo",
                Content = $"Tổng thanh toán: {FinalAmountTextBlock.Text}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await checkoutDialog.ShowAsync();
        }
    }
}
