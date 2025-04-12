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

        private async void BarcodeScanButton_Click(object sender, RoutedEventArgs e)
        {
            await ScanBarcodeFromCameraRealtime();
        }

        private async Task ScanBarcodeFromCameraRealtime()
        {
            MediaCapture mediaCapture = null;
            DispatcherTimer scanTimer = null;
            ContentDialog captureDialog = null;

            try
            {
                // Khởi tạo camera
                mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video
                };

                await mediaCapture.InitializeAsync(settings);

                // Tạo CaptureElement để hiển thị camera
                var captureElement = new MediaPlayerElement
                {
                    Width = 400,
                    Height = 300,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform
                };

                // Tạo TextBlock để hiển thị trạng thái
                var statusTextBlock = new TextBlock
                {
                    Text = "Đang quét barcode...",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                // Tạo StackPanel chứa camera preview và trạng thái
                var content = new StackPanel();
                content.Children.Add(captureElement);
                content.Children.Add(statusTextBlock);

                // Tạo dialog để hiển thị camera
                captureDialog = new ContentDialog
                {
                    Title = "Quét mã barcode",
                    Content = content,
                    CloseButtonText = "Hủy bỏ",
                    XamlRoot = this.Content.XamlRoot
                };

                // Kết nối camera với CaptureElement
                var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
                captureElement.SetMediaPlayer(mediaPlayer);

                // Kết nối camera với MediaPlayer
                var mediaSource = MediaSource.CreateFromMediaFrameSource(mediaCapture.FrameSources.First().Value);
                mediaPlayer.Source = mediaSource;

                // Bắt đầu hiển thị camera
                await mediaCapture.StartPreviewAsync();

                // Tạo timer để thực hiện quét barcode liên tục
                scanTimer = new DispatcherTimer();
                scanTimer.Interval = TimeSpan.FromMilliseconds(500); // Quét mỗi 500ms

                scanTimer.Tick += async (s, args) =>
                {
                    try
                    {
                        // Capture frame từ camera
                        var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(
                            ImageEncodingProperties.CreateJpeg());

                        var capturedPhoto = await lowLagCapture.CaptureAsync();
                        var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

                        // Giải phóng capture
                        await lowLagCapture.FinishAsync();

                        // Xử lý ảnh để tìm barcode
                        var scannedBarcode = ProcessBarcodeImage(softwareBitmap);

                        if (!string.IsNullOrEmpty(scannedBarcode))
                        {
                            // Cập nhật trạng thái
                            statusTextBlock.Text = $"Đã tìm thấy: {scannedBarcode}";

                            // Dừng timer và tắt camera
                            scanTimer.Stop();
                            await mediaCapture.StopPreviewAsync();

                            // Đóng dialog
                            captureDialog.Hide();

                            // Thiết lập giá trị tìm kiếm và thực hiện tìm kiếm
                            TextBoxSearch.Text = scannedBarcode;
                            await ApplyFilters(scannedBarcode);
                        }
                    }
                    catch (Exception ex)
                    {
                        statusTextBlock.Text = $"Lỗi: {ex.Message}";
                    }
                };

                // Bắt đầu quét
                scanTimer.Start();

                // Hiển thị dialog và đợi kết quả
                var dialogResult = await captureDialog.ShowAsync();

                // Nếu dialog đóng (người dùng ấn nút hủy), dừng timer và camera
                if (scanTimer.IsEnabled)
                {
                    scanTimer.Stop();
                }

                if (mediaCapture != null)
                {
                    await mediaCapture.StopPreviewAsync();
                }
            }
            catch (Exception ex)
            {
                // Đảm bảo dừng timer và camera trong trường hợp lỗi
                if (scanTimer != null && scanTimer.IsEnabled)
                {
                    scanTimer.Stop();
                }

                if (mediaCapture != null)
                {
                    try
                    {
                        if (mediaCapture.CameraStreamState == CameraStreamState.Streaming)
                        {
                            await mediaCapture.StopPreviewAsync();
                        }
                    }
                    catch (Exception innerEx)
                    {
                        Debug.WriteLine($"Lỗi khi dừng preview: {innerEx.Message}");
                    }
                }

                await ShowErrorDialog($"Lỗi khi quét barcode: {ex.Message}");
            }
        }

        private string ProcessBarcodeImage(SoftwareBitmap bitmap)
        {
            try
            {
                // Đảm bảo bitmap ở định dạng Bgra8
                if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8)
                {
                    bitmap = SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Lấy dữ liệu pixel
                byte[] pixelData = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
                bitmap.CopyToBuffer(pixelData.AsBuffer());

                // Tạo RGBLuminanceSource trực tiếp từ dữ liệu BGRA
                // ZXing sẽ tự động xử lý việc chuyển đổi sang luminance
                var luminanceSource = new RGBLuminanceSource(pixelData, bitmap.PixelWidth, bitmap.PixelHeight);

                // Tạo BinaryBitmap
                var binaryBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));

                // Thiết lập barcode reader
                var reader = new MultiFormatReader();
                var hints = new Dictionary<DecodeHintType, object>
                {
                    { DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat> {
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.UPC_A,
                        BarcodeFormat.UPC_E,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128
                    }},
                    { DecodeHintType.TRY_HARDER, true }
                };

                // Giải mã barcode
                var result = reader.decode(binaryBitmap, hints);

                return result?.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi xử lý hình ảnh barcode: {ex.Message}");
                return null;
            }
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
