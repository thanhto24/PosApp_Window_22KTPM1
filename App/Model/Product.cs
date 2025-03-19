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
        public static int autoIncreateCode = 1;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get; set; }
        public string Price { get; set; }
        public string ImagePath { get; set; }
        public string BarCode { get; set; }
        public int ID { get; set; }
        public string TypeGroup { get; set; }
        public float VAT { get; set; }
        public string CostPrice { get; set; }

        public Product(string name, string price, string image, string typeGroup, float vAT, string costPrice, string barCode)
        {
            Name = name;
            Price = price;
            ImagePath = image;
            ID = autoIncreateCode;
            TypeGroup = typeGroup;
            VAT = vAT;
            CostPrice = costPrice;
            BarCode = barCode;

            autoIncreateCode += 1;
        }

        public Product() { }
    }
}
