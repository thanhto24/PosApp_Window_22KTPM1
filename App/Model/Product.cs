using System;
using System.ComponentModel;

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
        }

        public Product() { }
    }
}
