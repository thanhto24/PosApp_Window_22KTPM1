using System;
using System.ComponentModel;

namespace App.Model
{
    public class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string ProductCode { get; set; }
        public string Name { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalPrice)); // Cập nhật TotalPrice
                }
            }
        }

        private int _price;
        public int Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                    OnPropertyChanged(nameof(TotalPrice)); // Cập nhật TotalPrice
                }
            }
        }

        public int TotalPrice => Price * Quantity; // Tính toán động

        public string ImagePath { get; set; }
        public string BarCode { get; set; }
        //public int Id { get; set; }
        public string TypeGroup { get; set; }
        public float Vat { get; set; }
        public int CostPrice { get; set; }

        public Product(string productCode, string name, int quantity, int price, int totalPrice, string image, string typeGroup, float vAT, int costPrice, string barCode)
        {
            ProductCode = productCode;
            Name = name;
            _quantity = quantity;
            _price = price;
            ImagePath = image;
            TypeGroup = typeGroup;
            Vat = vAT;
            CostPrice = costPrice;
            BarCode = barCode;
        }

        public Product() { }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
