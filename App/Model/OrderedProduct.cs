using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class OrderedProduct : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string productCode { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }

        public OrderedProduct(string ProductCode, string Name, int Quantity, decimal Price)
        {
            productCode = ProductCode;
            name = Name;
            quantity = Quantity;
            price = Price;
        }

        public OrderedProduct() { }
    }
}