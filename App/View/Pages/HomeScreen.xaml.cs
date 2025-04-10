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
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;

namespace App.View.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public CategoryViewModel CategoryViewModel { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public CartViewModel CartViewModel { get; set; }

        public VoucherViewModel VoucherViewModel { get; set; }
        public CustomerViewModel CustomerViewModel { get; set; }

        private double finalAmount = 0;
        public double vcDis = 0, cusDis = 0;
        public HomeScreen()
        {
            this.InitializeComponent();
            CategoryViewModel = new CategoryViewModel();
            ProductViewModel = new ProductViewModel();
            CartViewModel = new CartViewModel();

            VoucherViewModel = new VoucherViewModel();
            CustomerViewModel = new CustomerViewModel();

            if (CategoryViewModel.categories.Any() && ProductViewModel.Products.Any())
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
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                // Check if adding one more would exceed inventory
                if (product.Quantity + 1 > product.Inventory)
                {
                    ContentDialog inventoryDialog = new ContentDialog
                    {
                        Title = "Thông báo",
                        Content = $"Không đủ hàng trong kho. Chỉ còn {product.Inventory} sản phẩm.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await inventoryDialog.ShowAsync();
                    return;
                }

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
                if(product.Quantity > 0)
                {
                    product.Quantity--;
                    CartViewModel.RemoveFromCart(product);
                    OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
                    ApplyDiscount();
                }
            }
        }


        private void PromoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string promoCode = PromoCodeTextBox.Text.Trim();

            vcDis = VoucherViewModel.ApplyVoucher(promoCode) / 100;
            ApplyDiscount();
        }

        private void CustomerCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phone = CustomerCodeTextBox.Text.Trim();

            cusDis = CustomerViewModel.ApplyCusPhone(phone) / 100;
            ApplyDiscount();
        }

        private void ApplyDiscount()
        {
            double total = CartViewModel.getTotalAmount();
            double discountVc = 0, discountCustomer = 0;


            // Áp dụng mã khuyến mãi
            discountVc = total * vcDis;
            discountCustomer = total * cusDis;

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

            if (CartViewModel.getTotalAmount() == 0)
                return;

            string customerName = string.IsNullOrWhiteSpace(CustomerName.Text) ? "Khách vãng lai" : CustomerName.Text;
            CartViewModel.CreateNewOrder(CartViewModel.totalDiscount, customerName);


            string phone = CustomerCodeTextBox.Text.Trim();
            string name = CustomerName.Text.Trim();
            CustomerViewModel.storeData(phone, name, finalAmount);

            string promoCode = PromoCodeTextBox.Text.Trim();
            VoucherViewModel.des(promoCode);

            // Update inventory in database
            UpdateInventoryAfterSale();

            ContentDialog checkoutDialog = new ContentDialog
            {
                Title = "Thông báo",
                Content = $"Tổng thanh toán: {FinalAmountTextBlock.Text}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await checkoutDialog.ShowAsync();

            // Reset product quantities in UI
            foreach (var product in ProductViewModel.Products)
            {
                product.Quantity = 0;
            }
        }

        private void VatToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            CartViewModel.IsVATEnabled = true;
            ApplyDiscount();
        }

        private void VatToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CartViewModel.IsVATEnabled = false;
            ApplyDiscount();
        }

        private void UpdateInventoryAfterSale()
        {
            foreach (var cartItem in CartViewModel.CartItems)
            {
                // Find the product in the products list
                var product = ProductViewModel.Products.FirstOrDefault(p => p.BarCode == cartItem.Product.BarCode);
                if (product != null)
                {
                    // Update inventory
                    product.Inventory -= cartItem.Quantity;

                    // Update in database
                    var updateValues = new Dictionary<string, object>
                    {
                        { "Inventory", product.Inventory }
                    };

                    ProductViewModel._dao.Products.UpdateByQuery(
                        updateValues,
                        "BarCode = @BarCode",
                        new Dictionary<string, object> { { "BarCode", product.BarCode } }
                    );
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }
        private void SearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // Check if Enter key was pressed
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ApplyFilters();
            }
        }

        private async Task ApplyFilters()
        {
            try
            {
                var filter = new Dictionary<string, object>();
                var searchBox = this.FindName("TextBoxSearch") as TextBox;
                string searchText = "";

                if (searchBox != null && !string.IsNullOrEmpty(searchBox.Text))
                {
                    searchText = searchBox.Text;
                    filter.Add("Name", searchText + "%"); // Use % for partial matching
                }


                // Apply date filter if needed (requires additional implementation in the DAO)
                // filter.Add("DateFilter", new Dictionary<string, DateTime> { 
                //    { "StartDate", _startDate },
                //    { "EndDate", _endDate }
                // });

                // Default sort by name ascending
                Dictionary<string, int> sort = new Dictionary<string, int> { { "Name", 1 } };

                await ProductViewModel.NewFilter(filter, sort);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
                await ShowErrorDialog($"Lỗi: {ex.Message}");
            }
        }

        private async Task ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }
}
