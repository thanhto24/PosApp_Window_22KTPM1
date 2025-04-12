using System.ComponentModel;
using BarcodeStandard;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;

// using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using Windows.Storage.Streams;

namespace App.Model
{
    public class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //public string ProductCode { get; set; }
        public string Name { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

        public int TotalPrice { get; set; } // = Price * Inventory

        public string ImagePath { get; set; }
        public string BarCode { get; set; }
        //public int Id { get; set; }
        public string TypeGroup { get; set; }
        public float Vat { get; set; }
        public int CostPrice { get; set; }

        //giá trị tồn kho:
        public int Inventory { get; set; }

        public BitmapImage BarCodeBitmap { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Product( string name, int quantity, int price, int totalPrice, string image, string typeGroup, float vAT, int costPrice, string barCode, int inventory = 1)
        {
            //ProductCode = productCode;
            Name = name;
            Quantity = quantity;
            Price = price;
            ImagePath = image;
            TypeGroup = typeGroup;
            Vat = vAT;
            CostPrice = costPrice;
            BarCode = barCode;

            //tồn kho:
            this.Inventory = inventory;

            //sinh ảnh barcode trong này luôn:
            // 1. tạo bitmap trống để UI có reference ngay
            BarCodeBitmap = new BitmapImage();

            // 2. bắt đầu nạp ảnh barcode bất đồng bộ
            _ = LoadBarcodeAsync(barCode);
        }

        // ---------- hàm sinh barcode -------------
        public async Task LoadBarcodeAsync(string code)
        {
            try
            {
                Debug.WriteLine($"Đang tạo barcode cho: {code}");
                // Tạo BitmapImage mới
                var bitmapImage = new BitmapImage();

                // Tạo barcode
                Barcode bc = new Barcode();

                // Sử dụng màu đen cho foreground và màu trắng (không phải trong suốt) cho background
                var blackColor = new SkiaSharp.SKColorF(0, 0, 0); // Đen
                var whiteColor = new SkiaSharp.SKColorF(1, 1, 1); // Trắng

                // Tạo hình ảnh barcode lớn hơn để dễ nhìn
                using var img = bc.Encode(BarcodeStandard.Type.Code128, code,
                                      blackColor, whiteColor,
                                      200, 100); // Kích thước lớn hơn

                // Mã hóa thành PNG với chất lượng cao
                using var data = img.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);

                // Tạo MemoryStream
                using MemoryStream ms = new MemoryStream();
                using (var stream = data.AsStream())
                {
                    await stream.CopyToAsync(ms);
                }

                // Đặt lại vị trí
                ms.Position = 0;

                // Tạo RandomAccessStream từ MemoryStream
                using IRandomAccessStream randomAccessStream = ms.AsRandomAccessStream();

                // Đặt nguồn cho BitmapImage
                await bitmapImage.SetSourceAsync(randomAccessStream);

                // Gán BitmapImage cho thuộc tính
                BarCodeBitmap = bitmapImage;

                // Thông báo thuộc tính đã thay đổi
                OnPropertyChanged(nameof(BarCodeBitmap));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi tạo barcode: {ex.Message}");
                BarCodeBitmap = null;
                OnPropertyChanged(nameof(BarCodeBitmap));
            }
            Debug.WriteLine($"Đã tạo barcode, BitmapImage là null: {BarCodeBitmap == null}");
        }

        public Product() { }
    }
}
