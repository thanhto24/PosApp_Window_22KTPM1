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
using static App.Service.MockDao;

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
            OrderedProducts.Clear();
            PromoCodeTextBox.Text = "";
            CustomerCodeTextBox.Text = "";
            ApplyDiscount();
            UpdateOrderSummary();
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ApplyDiscount();
            var newOrder = CreateNewOrder();

            if (mockDao.Orders is MockDao.MockOrderRepository orderRepo)
            {
                orderRepo.Insert(newOrder);
                List<Order_> updatedOrders = orderRepo.GetAll();

                Debug.WriteLine("===== Danh sách đơn hàng sau khi thêm mới =====");
                foreach (var order in updatedOrders)
                {
                    Debug.WriteLine($"ID: {order.Id}, Invoice: {order.InvoiceCode}, Khách: {order.Customer}, Tổng tiền: {order.TotalAmount}");
                    Debug.WriteLine("Sản phẩm đã đặt:");
                    foreach (var product in order.OrderedProducts)
                    {
                        Debug.WriteLine($"- {product.Name}: {product.Quantity} x {product.Price:N0}đ");
                    }
                }
            }

            ContentDialog checkoutDialog = new ContentDialog
            {
                Title = "Thông báo",
                Content = $"Tổng thanh toán: {FinalAmountTextBlock.Text}",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await checkoutDialog.ShowAsync();

            ClearCart_Click(null, null);
            Frame.Navigate(typeof(AllOrdersPage));
        }



        private Order_ CreateNewOrder()
        {
            // Lấy danh sách đơn hàng từ MockOrderRepository
            MockOrderRepository orderRepo = new MockOrderRepository();
            List<Order_> orders = orderRepo.GetAll();

            // Xác định ID mới dựa trên ID lớn nhất hiện có
            int newId = (orders.Count > 0) ? orders.Max(o => o.Id) + 1 : 1;
            string invoiceId = $"INV{newId:D3}"; // Mã hóa đơn tăng dần

            // Lấy tên khách hàng từ TextBox
            string customerName = string.IsNullOrWhiteSpace(CustomerName.Text) ? "Khách vãng lai" : CustomerName.Text.Trim();
            List<Product> orderedProductsList = OrderedProducts.ToList();
            decimal totalAmount = (decimal)CartViewModel.getTotalAmount();
            decimal discount = decimal.Parse(DiscountAmountTextBlock.Text.Replace("đ", "").Replace("-", "").Trim());
            decimal finalAmount = totalAmount - discount;
            decimal cashReceived = finalAmount;

            return new Order_(
                id: newId,
                invoiceCode: invoiceId,
                customer: customerName,
                saleDateTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                orderedProducts: orderedProductsList,
                totalAmount: totalAmount,
                totalDiscount: discount,
                totalPayment: finalAmount,
                totalCost: cashReceived,
                paymentMethod: "Tiền mặt",
                status: "Đã giao",
                paymentStatus: "Đã thanh toán",
                notes: "Giao hàng thành công"
            );
        }


        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine($"New Height: {e.NewSize.Height}");

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

            Debug.WriteLine($"OrderScrollViewer MaxHeight: {OrderScrollViewer.MaxHeight}");
        }


    }
}
