using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class CartItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public string QuantityText => $"Số lượng: {Quantity}";

    }
}
