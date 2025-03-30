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

        public string ProductCode { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderedProduct(string productCode, string name, int quantity, decimal price)
        {
            ProductCode = productCode;
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        public OrderedProduct() { }
    }
}