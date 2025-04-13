using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using QRCoder;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static SkiaSharp.HarfBuzz.SKShaper;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.View.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class QRPaymentWindow : Window
    {
        public QRPaymentWindow(string qrData, long orderCode, int amount, string checkoutUrl)
        {
            InitializeComponent();

            // Tạo ảnh QR
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            stream.WriteAsync(qrCodeBytes.AsBuffer());
            stream.Seek(0);

            BitmapImage qrImage = new BitmapImage();
            qrImage.SetSourceAsync(stream);


            MainPanel.Children.Add(new Image { Source = qrImage, Width = 200, Height = 200 });
            MainPanel.Children.Add(new TextBlock { Text = $"Mã đơn hàng: {orderCode}", TextAlignment = TextAlignment.Center });
            MainPanel.Children.Add(new TextBlock { Text = $"Số tiền: {amount:N0} VND", TextAlignment = TextAlignment.Center });

            var button = new Button { Content = "Mở trang thanh toán" };
            button.Click += (s, e) => Process.Start(new ProcessStartInfo(checkoutUrl) { UseShellExecute = true });

            MainPanel.Children.Add(button);
        }
    }

}
