using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class Customer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; set; }
        public string Phone_num { get; set; }
        public int AmountOrder { get; set; }
        public decimal TotalPaid { get; set; }
        public string Rank { get; set; }

        public Customer(string name, string Phone_num, int amountOrder, decimal totalPaid, string rank)
        {
            this.Name = name;
            this.Phone_num = Phone_num;
            this.AmountOrder = amountOrder;
            this.TotalPaid = totalPaid;
            this.Rank = rank;
        }

        public Customer() { }
    }
}
