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
        public string Price { get; set; }
        public string ImagePath { get; set; }
        public Product(string name, string price, string image)
        {
            Name = name;
            Price = price;
            ImagePath = image;
        }

        public Product() { }
    }
}
