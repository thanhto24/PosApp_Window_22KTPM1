using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.Model;
using App.View.ViewModel;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Input;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Graphics.Imaging;
using ZXing.Common;
using ZXing;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using ZXing.OneD;
using App.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.UI.Text;
using static QRCoder.PayloadGenerator;
using System.Text.Json;
using QRCoder;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Core;
using static App.Service.Payos;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using App.View.Dialogs;
using Windows.Web.Http;
using static App.Service.GHN;
using static App.Service.GHN.ShipService;

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
        public double vatFee = 0, shipFee = 0;
        public bool isShip = false;
        public CreateOrderRequest orderShip = new CreateOrderRequest();
        public ShipService shipService;
        public string orderCodeCreated = "";
        public HomeScreen()
        {
            this.InitializeComponent();
            CategoryViewModel = new CategoryViewModel();
            ProductViewModel = new ProductViewModel();
            CartViewModel = new CartViewModel();

            VoucherViewModel = new VoucherViewModel();
            CustomerViewModel = new CustomerViewModel();
            shipService = new ShipService();
            if (CategoryViewModel.categories.Any() && ProductViewModel.Products.Any())
                ProductViewModel.LoadProductsByCategory(CategoryViewModel.categories[0].Name);

            ApplyDiscount();
        }

        private async void BarcodeScanButton_Click(object sender, RoutedEventArgs e)
        {
            var scanner = new CameraScanner(ProductViewModel._window);

            // Đăng ký sự kiện xử lý khi quét barcode thành công
            scanner.OnBarcodeScanSuccess += async (barcodeText) =>
            {
                // Thiết lập giá trị tìm kiếm và thực hiện tìm kiếm
                TextBoxSearch.Text = barcodeText;
                await ApplyFilters(barcodeText);
            };

            // Khởi động quét barcode
            await scanner.ScanBarcodeFromCamera(this.Content.XamlRoot);
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
                if (product.Quantity > 0)
                {
                    product.Quantity--;
                    CartViewModel.RemoveFromCart(product);
                    OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
                    ApplyDiscount();
                }
            }
        }

        private void checkVoucher()
        {
            string promoCode = PromoCodeTextBox.Text.Trim();

            vcDis = VoucherViewModel.ApplyVoucher(promoCode, CartViewModel.getTotalAmount()) / 100;
        }

        private void PromoCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyDiscount();
        }

        private void CustomerCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phone = CustomerCodeTextBox.Text.Trim();

            //cusDis = CustomerViewModel.ApplyCusPhone(phone) / 100;
            cusDis = CustomerViewModel.ApplyCusPhone(phone);
            ApplyDiscount();
        }

        private void ApplyDiscount()
        {
            double total = CartViewModel.getTotalAmount();
            double discountVc = 0, discountCustomer = 0;
            double otherFee = vatFee + shipFee;
            checkVoucher();

            // Áp dụng mã khuyến mãi
            discountVc = total * vcDis;
            discountCustomer = total * cusDis;

            finalAmount = total + otherFee - discountVc - discountCustomer;
            CartViewModel.totalDiscount = discountVc + discountCustomer;

            // Hiển thị số tiền trên giao diện
            TotalAmountTextBlock.Text = $"{total:N0}đ";
            OtherFeeTextBlock.Text = $"{otherFee:N0}đ";
            DiscountAmountTextBlock.Text = $"-{discountVc + discountCustomer:N0}đ";
            FinalAmountTextBlock.Text = $"{finalAmount:N0}đ";
        }

        private void ClearCart_Click(object sender, RoutedEventArgs e)
        {
            CartViewModel.Clear_();
            PromoCodeTextBox.Text = "";
            CustomerCodeTextBox.Text = "";

            ProductViewModel.ClearAllQuantities();

            ApplyDiscount();
            OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ClearCart_Click(null, null);
        }

        private async Task Checkout(string paymentType)
        {
            ApplyDiscount();

            if (CartViewModel.getTotalAmount() == 0)
            {
                await ShowErrorDialog("Vui lòng thêm sản phẩm");
                return;
            }

            string phone = CustomerCodeTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(phone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9,10}$"))
                {
                    await ShowErrorDialog("Vui lòng nhập số điện thoại hợp lệ");
                    return;
                }
            }

            string customerName = string.IsNullOrWhiteSpace(CustomerName.Text) ? "Khách vãng lai" : CustomerName.Text;

            if (isShip)
            {
                if (CustomerName.Text == "" || CustomerCodeTextBox.Text == "")
                {
                    await ShowErrorDialog("Tên khách hàng, số điện thoại không được để trống khi ship");
                    return;
                }
                bool canShip = await CreateOrderShipFunc();
                if (!canShip)
                {
                    Clear_All();
                    return;
                }
                CartViewModel.CreateNewOrder(CartViewModel.totalDiscount, customerName, "COD", "Đang giao hàng", "Thanh toán khi nhận hàng", "GHN: " + orderCodeCreated);

                orderCodeCreated = "";
            }
            else
                CartViewModel.CreateNewOrder(CartViewModel.totalDiscount, customerName, paymentType);







            string name = CustomerName.Text.Trim();
            CustomerViewModel.storeData(phone, name, finalAmount);

            string promoCode = PromoCodeTextBox.Text.Trim();
            if (this.vcDis > 0)
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
            Clear_All();

        }

        private void Clear_All()
        {
            // Reset product quantities in UI

            foreach (var product in ProductViewModel.Products)
            {
                product.Quantity = 0;
            }

            CartViewModel.Clear_();
            PromoCodeTextBox.Text = "";
            CustomerCodeTextBox.Text = "";
            CustomerName.Text = "";
            vatFee = 0;
            shipFee = 0;
            ApplyDiscount();
            OrderSummaryText.Text = $"Số lượng: {CartViewModel.getTotalQuantity().ToString()} món";
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            await Checkout("Tiền mặt");
        }

        private void VatToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //CartViewModel.IsVATEnabled = true;
            vatFee = CartViewModel.getVATFee();
            ApplyDiscount();
        }

        private void VatToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //CartViewModel.IsVATEnabled = false;
            vatFee = 0;
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

        private async void QR_Click(object sender, RoutedEventArgs e)
        {
            var paymentService = new PaymentService();
            int amount = (int)Math.Floor(this.finalAmount);

            try
            {
                var result = await paymentService.CreatePayment(amount);
                if (result == null)
                {
                    await ShowErrorDialog("Không tạo được mã thanh toán.");
                    return;
                }

                // Tạo dialog và hiển thị nội dung QR như bạn làm trước đó...
                ContentDialog qrDialog = new ContentDialog
                {
                    Title = "Quét mã QR để thanh toán",
                    XamlRoot = this.XamlRoot,
                    CloseButtonText = "Đóng",
                    DefaultButton = ContentDialogButton.Close,
                };

                StackPanel contentPanel = new StackPanel { Spacing = 15, HorizontalAlignment = HorizontalAlignment.Center };

                // Tạo ảnh QR
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(result.qrCode, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(qrCodeBytes.AsBuffer());
                stream.Seek(0);

                BitmapImage qrImage = new BitmapImage();
                await qrImage.SetSourceAsync(stream);

                contentPanel.Children.Add(new Image { Source = qrImage, Width = 250, Height = 250 });

                contentPanel.Children.Add(new TextBlock { Text = $"Mã đơn hàng: {result.orderCode}", TextAlignment = TextAlignment.Center });
                contentPanel.Children.Add(new TextBlock { Text = $"Số tiền: {amount:N0} VND", TextAlignment = TextAlignment.Center });

                Button openUrlButton = new Button
                {
                    Content = "Mở trang thanh toán",
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                openUrlButton.Click += async (s2, e2) =>
                {
                    await Launcher.LaunchUriAsync(new Uri(result.checkoutUrl));
                };
                contentPanel.Children.Add(openUrlButton);

                qrDialog.Content = contentPanel;


                _ = qrDialog.ShowAsync(); // không await để dialog không block luồng

                bool paid = false;
                int elapsed = 0;
                int timeout = 5 * 60 * 1000; // 5 phút (milliseconds)
                int checkInterval = 3000; // kiểm tra mỗi 3 giây

                while (!paid && elapsed < timeout)
                {
                    await Task.Delay(checkInterval);
                    elapsed += checkInterval;

                    paid = await paymentService.CheckPaymentStatus(result.orderCode);
                }

                qrDialog.Hide(); // đóng dialog dù thành công hay không

                if (paid)
                {
                    await Checkout("QR");
                }
                else
                {
                    await ShowErrorDialog("Thanh toán không thành công hoặc đã hết thời gian chờ.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog($"Lỗi: {ex.Message}");
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

        private async Task ApplyFilters(string barcode = null)
        {
            try
            {
                var filter = new Dictionary<string, object>();

                // Nếu có barcode, ưu tiên tìm theo barcode
                if (!string.IsNullOrEmpty(barcode))
                {
                    filter.Add("BarCode", barcode);
                }
                else if (TextBoxSearch != null && !string.IsNullOrEmpty(TextBoxSearch.Text))
                {
                    // Nếu không có barcode, tìm theo text
                    string searchText = TextBoxSearch.Text;
                    filter.Add("Name", searchText + "%"); // Sử dụng % cho tìm kiếm một phần
                }

                // Sắp xếp mặc định theo tên
                Dictionary<string, int> sort = new Dictionary<string, int> { { "Name", 1 } };

                await ProductViewModel.NewFilter(filter, sort);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi áp dụng bộ lọc: {ex.Message}");
                await ShowErrorDialog($"Lỗi: {ex.Message}");
            }
        }
        private async void ShipOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddressSelectionDialog();
            dialog.XamlRoot = this.XamlRoot;

            // Mở ContentDialog và chờ kết quả
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                isShip = true;

                var toProvince = dialog.ToAddress.Province.id; // Địa chỉ nhận
                var toDistrict = dialog.ToAddress.District.id;
                var toWard = dialog.ToAddress.Ward.id;
                var toDetailedAddress = dialog.ToAddress.Detailed;


                orderShip.to_district_id = dialog.ToAddress.District.id;
                orderShip.to_ward_code = dialog.ToAddress.Ward.id.ToString();
                orderShip.to_address = dialog.ToAddress.Detailed;

                shipFee = await GetShipFeeAsync(orderShip.to_ward_code, orderShip.to_district_id);
                if (shipFee == -1)
                {
                    shipFee = 0;
                    isShip = false;
                    await ShowErrorDialog("Rất tiếc, hiện GHN không hỗ trợ giao hàng ở vị trí này");
                    ShipToggleButton.IsChecked = false;
                }
            }
            else
            {
                isShip = false;
                shipFee = 0;
            }
            ApplyDiscount();
        }

        private void ShipOrder_UnClick(object sender, RoutedEventArgs e)
        {
            shipFee = 0;
            isShip = false;
        }

        private async Task<bool> CreateOrderShipFunc()
        {
            CreateOrderRequest order = this.orderShip;
            order.to_phone = CustomerCodeTextBox.Text;
            order.to_name = CustomerName.Text;

            OrderShipResponse order_create = await shipService.CreateOrderShip(order);
            if (order_create != null && order_create.total_fee != -1)
            {
                orderCodeCreated = order_create.order_code;
                await ShowDialogFunc("Tạo thành công đơn ship GHN\n", "Code: " + order_create.order_code + "\nPhí vận chuyển: " + order_create.total_fee);
                return true;
            }
            else
            {
                if (order_create == null)
                {
                    await ShowErrorDialog("Không tạo được đơn hàng, vui lòng thử lại");
                }
                else
                {
                    await ShowErrorDialog(order_create.order_code);
                }
                return false;
            }
        }

        private async Task ShowDialogFunc(string Title, string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = Title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
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
