using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class CustomerViewModel
    {
        private IDao _dao;

        public FullObservableCollection<Customer> customers { get; set; }
        public FullObservableCollection<Customer> displayCustomers { get; set; }

        public CustomerViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Customer> list = _dao.Customers.GetAll();
            customers = new FullObservableCollection<Customer>(list);
            displayCustomers = new FullObservableCollection<Customer>(list);
        }

        public async void findByPhone(string phone)
        {
            this.displayCustomers.Clear();
            Customer found = this.customers[0];
            this.displayCustomers.Add(found);
        }

        public async void resetClick()
        {
            this.displayCustomers.Clear();
            foreach (var customer in this.customers)
            {
                this.displayCustomers.Add(customer);
            }
        }
    }
}
