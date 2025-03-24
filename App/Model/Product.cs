using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class Product : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get; set; }
        public int Price { get; set; }
        public string ImagePath { get; set; }
        public string BarCode { get; set; }
        public int Id { get; set; }
        public string TypeGroup { get; set; }
        public float VAT { get; set; }
        public int CostPrice { get; set; }

        public Product(string name, int price, string image, string typeGroup, float vAT, int costPrice, string barCode)
        {
            Name = name;
            Price = price;
            ImagePath = image;
            TypeGroup = typeGroup;
            VAT = vAT;
            CostPrice = costPrice;
            BarCode = barCode;
        }

        public Product() { }
    }
}
