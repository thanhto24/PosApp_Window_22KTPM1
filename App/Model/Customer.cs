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

        public string name { get; set; }
        public string phone_num { get; set; }
        public int amountOrder { get; set; }
        public decimal totalPaid { get; set; }
        public string rank { get; set; }

        public Customer(string name, string phone_num, int amountOrder, decimal totalPaid, string rank)
        {
            this.name = name;
            this.phone_num = phone_num;
            this.amountOrder = amountOrder;
            this.totalPaid = totalPaid;
            this.rank = rank;
        }

        public Customer() { }
    }
}
