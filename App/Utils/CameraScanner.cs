using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using ZXing;
using ZXing.Common;
using ZXing.OneD;
using System.Runtime.InteropServices.WindowsRuntime;

namespace App.Utils
{
    public class CameraScanner
    {
        // Delegate cho sự kiện khi quét thành công
        public delegate void BarcodeScanSuccessHandler(string barcodeText);

        // Event khi quét mã vạch thành công
        public event BarcodeScanSuccessHandler OnBarcodeScanSuccess;

        // Cửa sổ ứng dụng hiện tại - cần thiết cho việc chọn file
        private Window _currentWindow;

        public CameraScanner(Window currentWindow)
        {
            _currentWindow = currentWindow;
        }

        public async Task ScanBarcodeFromCamera(XamlRoot xamlRoot)
        {
            MediaCapture mediaCapture = null;
            ContentDialog captureDialog = null;
            DispatcherTimer timer = null;
            bool isBusy = false;

            try
            {
                // Khởi tạo camera
                mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    MediaCategory = MediaCategory.Other,
                    AudioProcessing = AudioProcessing.Default
                };

                await mediaCapture.InitializeAsync(settings);

                // Tạo Image element để hiển thị camera
                var imageElement = new Image
                {
                    Width = 400,
                    Height = 300,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform
                };

                // Tạo TextBlock để hiển thị trạng thái
                var statusTextBlock = new TextBlock
                {
                    Text = "Đặt mã barcode vào khung hình và chụp",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                // Tạo nút chụp ảnh
                var captureButton = new Button
                {
                    Content = "Chụp ảnh",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                // Tạo nút chọn ảnh từ thiết bị
                var selectImageButton = new Button
                {
                    Content = "Chọn ảnh từ thiết bị",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                // Tạo StackPanel chứa camera preview, trạng thái và các nút
                var content = new StackPanel();
                content.Children.Add(imageElement);
                content.Children.Add(statusTextBlock);

                // Tạo StackPanel ngang để chứa các nút
                var buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 10,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                buttonsPanel.Children.Add(captureButton);
                buttonsPanel.Children.Add(selectImageButton);
                content.Children.Add(buttonsPanel);

                // Tạo dialog để hiển thị camera
                captureDialog = new ContentDialog
                {
                    Title = "Quét mã barcode",
                    Content = content,
                    CloseButtonText = "Hủy bỏ",
                    XamlRoot = xamlRoot
                };

                // Tạo một hàm riêng để cập nhật preview
                async Task UpdatePreview()
                {
                    if (isBusy) return;

                    try
                    {
                        isBusy = true;

                        // Chỉ cập nhật preview, không xử lý barcode
                        var imageProperties = ImageEncodingProperties.CreateJpeg();

                        using (var stream = new InMemoryRandomAccessStream())
                        {
                            await mediaCapture.CapturePhotoToStreamAsync(imageProperties, stream);
                            stream.Seek(0);

                            // Tạo BitmapImage từ stream
                            var bitmapImage = new BitmapImage();
                            await bitmapImage.SetSourceAsync(stream);

                            // Cập nhật Image
                            imageElement.Source = bitmapImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Chỉ ghi log lỗi, không hiển thị cho người dùng để tránh làm gián đoạn trải nghiệm
                        Debug.WriteLine($"Lỗi cập nhật preview: {ex.Message}");
                    }
                    finally
                    {
                        isBusy = false;
                    }
                }

                // Hiển thị camera preview với khoảng thời gian dài hơn
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(500); // Tăng lên 500ms
                timer.Tick += async (s, args) => await UpdatePreview();

                // Xử lý sự kiện khi người dùng nhấn nút chụp ảnh
                captureButton.Click += async (s, args) =>
                {
                    // Dừng timer preview
                    timer.Stop();

                    // Cập nhật trạng thái
                    statusTextBlock.Text = "Đang xử lý...";
                    captureButton.IsEnabled = false;
                    selectImageButton.IsEnabled = false;

                    try
                    {
                        // Đảm bảo không có hoạt động camera nào đang diễn ra
                        await Task.Delay(100);

                        // Chụp ảnh
                        var imageProperties = ImageEncodingProperties.CreateJpeg();

                        using (var stream = new InMemoryRandomAccessStream())
                        {
                            await mediaCapture.CapturePhotoToStreamAsync(imageProperties, stream);
                            stream.Seek(0);

                            // Sử dụng BitmapDecoder để tạo bitmap cho việc xử lý barcode
                            var decoder = await BitmapDecoder.CreateAsync(stream);
                            var bitmap = await decoder.GetSoftwareBitmapAsync();

                            if (bitmap != null)
                            {
                                // Xử lý ảnh để tìm barcode
                                var scannedBarcode = ProcessBarcodeImage(bitmap);

                                if (!string.IsNullOrEmpty(scannedBarcode))
                                {
                                    // Cập nhật trạng thái
                                    statusTextBlock.Text = $"Đã tìm thấy: {scannedBarcode}";

                                    // Đóng dialog sau một khoảng thời gian ngắn
                                    await Task.Delay(1000);
                                    captureDialog.Hide();

                                    // Kích hoạt sự kiện quét thành công
                                    OnBarcodeScanSuccess?.Invoke(scannedBarcode);
                                    return;
                                }
                                else
                                {
                                    // Không tìm thấy barcode
                                    statusTextBlock.Text = "Không tìm thấy mã barcode. Vui lòng thử lại.";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        statusTextBlock.Text = $"Lỗi: {ex.Message}";
                    }

                    // Bật lại nút và tiếp tục preview
                    captureButton.IsEnabled = true;
                    selectImageButton.IsEnabled = true;
                    timer.Start();
                };

                // Xử lý sự kiện khi người dùng nhấn nút chọn ảnh từ thiết bị
                selectImageButton.Click += async (s, args) =>
                {
                    // Dừng timer preview
                    timer.Stop();

                    try
                    {
                        // Cập nhật trạng thái
                        statusTextBlock.Text = "Đang chọn ảnh...";
                        captureButton.IsEnabled = false;
                        selectImageButton.IsEnabled = false;

                        // Tạo FileOpenPicker để chọn ảnh
                        var picker = new FileOpenPicker();

                        // Thiết lập WinRT context cho FileOpenPicker
                        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_currentWindow);
                        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                        // Thiết lập các loại file được chọn
                        picker.ViewMode = PickerViewMode.Thumbnail;
                        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        picker.FileTypeFilter.Add(".jpg");
                        picker.FileTypeFilter.Add(".jpeg");
                        picker.FileTypeFilter.Add(".png");
                        picker.FileTypeFilter.Add(".bmp");

                        // Mở picker và đợi người dùng chọn file
                        var file = await picker.PickSingleFileAsync();

                        if (file != null)
                        {
                            // Cập nhật trạng thái
                            statusTextBlock.Text = "Đang xử lý ảnh...";

                            // Đọc file ảnh
                            using (var stream = await file.OpenReadAsync())
                            {
                                // Hiển thị ảnh đã chọn
                                var bitmapImage = new BitmapImage();
                                await bitmapImage.SetSourceAsync(stream);
                                imageElement.Source = bitmapImage;

                                // Đặt lại stream về đầu để đọc lại
                                stream.Seek(0);

                                // Tạo bitmap từ stream
                                var decoder = await BitmapDecoder.CreateAsync(stream);
                                var bitmap = await decoder.GetSoftwareBitmapAsync();

                                if (bitmap != null)
                                {
                                    // Xử lý ảnh để tìm barcode
                                    var scannedBarcode = ProcessBarcodeImage(bitmap);

                                    if (!string.IsNullOrEmpty(scannedBarcode))
                                    {
                                        // Cập nhật trạng thái
                                        statusTextBlock.Text = $"Đã tìm thấy: {scannedBarcode}";

                                        // Đóng dialog sau một khoảng thời gian ngắn
                                        await Task.Delay(1000);
                                        captureDialog.Hide();

                                        // Kích hoạt sự kiện quét thành công
                                        OnBarcodeScanSuccess?.Invoke(scannedBarcode);
                                        return;
                                    }
                                    else
                                    {
                                        // Không tìm thấy barcode
                                        statusTextBlock.Text = "Không tìm thấy mã barcode trong ảnh. Vui lòng thử lại.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Người dùng đã hủy việc chọn file
                            statusTextBlock.Text = "Đặt mã barcode vào khung hình và chụp";
                        }
                    }
                    catch (Exception ex)
                    {
                        statusTextBlock.Text = $"Lỗi: {ex.Message}";
                    }

                    // Bật lại nút và tiếp tục preview
                    captureButton.IsEnabled = true;
                    selectImageButton.IsEnabled = true;
                    timer.Start();
                };

                // Xử lý sự kiện đóng dialog
                captureDialog.Closing += (s, args) =>
                {
                    // Dừng timer khi dialog đóng
                    if (timer != null && timer.IsEnabled)
                    {
                        timer.Stop();
                    }
                };

                // Bắt đầu preview sau khi dialog hiển thị
                captureDialog.Opened += async (s, args) =>
                {
                    // Cập nhật preview một lần trước khi bắt đầu timer
                    await UpdatePreview();
                    timer.Start();
                };

                // Hiển thị dialog và đợi kết quả
                await captureDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorDialog($"Lỗi khi quét barcode: {ex.Message}", xamlRoot);
            }
            finally
            {
                // Dừng timer nếu còn chạy
                if (timer != null && timer.IsEnabled)
                {
                    timer.Stop();
                }

                // Đảm bảo giải phóng tài nguyên camera
                if (mediaCapture != null)
                {
                    try
                    {
                        mediaCapture.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Lỗi khi giải phóng tài nguyên camera: {ex.Message}");
                    }
                }
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

                // Chuyển đổi dữ liệu BGRA sang mảng byte cho ZXing
                byte[] luminanceData = new byte[bitmap.PixelWidth * bitmap.PixelHeight];
                for (int i = 0; i < bitmap.PixelHeight; i++)
                {
                    for (int j = 0; j < bitmap.PixelWidth; j++)
                    {
                        int pixelIndex = (i * bitmap.PixelWidth + j) * 4;
                        // Chuyển BGR thành luminance: 0.299R + 0.587G + 0.114B
                        luminanceData[i * bitmap.PixelWidth + j] = (byte)(
                            (0.299 * pixelData[pixelIndex + 2]) +  // R
                            (0.587 * pixelData[pixelIndex + 1]) +  // G
                            (0.114 * pixelData[pixelIndex])        // B
                        );
                    }
                }

                // Tạo luminance source từ dữ liệu đã xử lý
                var luminanceSource = new RGBLuminanceSource(luminanceData, bitmap.PixelWidth, bitmap.PixelHeight, true);

                // Tạo BinaryBitmap với nhiều loại binarizer để thử nghiệm
                var binaryBitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));

                // Chỉ sử dụng Code 128 reader cho hiệu suất tốt hơn
                var reader = new Code128Reader();

                // Thử decode trực tiếp với Code128Reader
                var result = reader.decode(binaryBitmap);

                // Nếu không thành công, thử lại với các tùy chọn khác
                if (result == null)
                {
                    // Thử với GlobalHistogramBinarizer
                    binaryBitmap = new BinaryBitmap(new GlobalHistogramBinarizer(luminanceSource));
                    result = reader.decode(binaryBitmap);

                    // Nếu vẫn không thành công, thử với MultiFormatReader và cấu hình cụ thể
                    if (result == null)
                    {
                        var multiReader = new MultiFormatReader();
                        var hints = new Dictionary<DecodeHintType, object>
                        {
                            { DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat> { BarcodeFormat.CODE_128 } },
                            { DecodeHintType.TRY_HARDER, true },
                            { DecodeHintType.PURE_BARCODE, true }
                        };

                        result = multiReader.decode(binaryBitmap, hints);
                    }
                }

                return result?.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi xử lý hình ảnh barcode: {ex.Message}");
                return null;
            }
        }

        private async Task ShowErrorDialog(string message, XamlRoot xamlRoot)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = xamlRoot
            };

            await errorDialog.ShowAsync();
        }
    }
}