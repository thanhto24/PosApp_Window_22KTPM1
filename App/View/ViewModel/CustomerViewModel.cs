using System;
using System.Collections.Generic;
using System.Linq;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class CustomerViewModel
    {
        private readonly IDao _dao;

        public FullObservableCollection<Customer> customers { get; set; }
        public FullObservableCollection<Customer> displayCustomers { get; set; }

        public CustomerViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Customer> list = _dao.Customers.GetAll();
            customers = new FullObservableCollection<Customer>(list);
            displayCustomers = new FullObservableCollection<Customer>(list);
        }

        public void findByPhone(string phone)
        {
            displayCustomers.Clear();

            var foundCustomers = customers.Where(c => c.Phone_num != null && c.Phone_num.Contains(phone));
            foreach (var customer in foundCustomers)
            {
                displayCustomers.Add(customer);
            }
        }


        public void resetClick()
        {
            displayCustomers.Clear();
            foreach (var customer in customers)
            {
                displayCustomers.Add(customer);
            }
        }

        public double ApplyCusPhone(string phone)
        {
            var filter = new Dictionary<string, object> { { "Phone_num", phone } };
            var cus = _dao.Customers.GetByQuery(filter);

            if (cus == null || !cus.Any())
                return 0;

            return cus[0].Rank switch
            {
                "New User" => 0.1,
                "Silver" => 0.2,
                "Gold" => 0.3,
                _ => 0
            };
        }

        public void storeData(string phone, string name, double totalAmount)
        {
            //System.Diagnostics.Debug.WriteLine(("Call store new cus" + phone));

            var filter = new Dictionary<string, object> { { "Phone_num", phone } };
            var cus = _dao.Customers.GetByQuery(filter);

            if (cus == null || !cus.Any())
            {
                if (phone == "")
                    return;
                if (name == "")
                    name = "Unknown";
                var newCustomer = new Customer(name, phone, 1, (decimal)totalAmount, "New User");
                _dao.Customers.Insert(newCustomer);
                customers.Add(newCustomer);
                //System.Diagnostics.Debug.WriteLine(("Insert new cus" + phone));
            }
            else
            {
                var foundCustomer = cus[0];
                int newAmount = foundCustomer.AmountOrder + 1;
                decimal newTotalPaid = foundCustomer.TotalPaid + (decimal)totalAmount;

                string newRank = foundCustomer.Rank;
                if (newAmount >= 10)
                    newRank = "Gold";
                else if (newAmount >= 5)
                    newRank = "Silver";

                var updateValues = new Dictionary<string, object>
                {
                    { "Phone_num", phone },
                    { "AmountOrder", newAmount },
                    { "TotalPaid", newTotalPaid },
                    { "Rank", newRank }
                };
                if (name != "")
                {
                    if (foundCustomer.Name == "Unknown")
                        updateValues.Add("Name", name);
                }
                _dao.Customers.UpdateByQuery(updateValues, "Phone_num = @phone", new Dictionary<string, object> { { "phone", phone } });

                customers.Remove(foundCustomer);
                customers.Add(new Customer(name, phone, newAmount, newTotalPaid, newRank));
                //System.Diagnostics.Debug.WriteLine(("Update new cus" + phone));

            }

            resetClick();
        }
    }
}
