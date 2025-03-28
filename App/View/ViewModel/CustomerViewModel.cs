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

            var foundCustomer = customers.FirstOrDefault(c => c.phone_num == phone);
            if (foundCustomer != null)
            {
                displayCustomers.Add(foundCustomer);
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
            var filter = new Dictionary<string, object> { { "phone_num", phone } };
            var cus = _dao.Customers.GetByQuery(filter);

            if (cus == null || !cus.Any())
                return 0;

            return cus[0].rank switch
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

            var filter = new Dictionary<string, object> { { "phone_num", phone } };
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
                int newAmount = foundCustomer.amountOrder + 1;
                decimal newTotalPaid = foundCustomer.totalPaid + (decimal)totalAmount;

                string newRank = foundCustomer.rank;
                if (newAmount >= 10)
                    newRank = "Gold";
                else if (newAmount >= 5)
                    newRank = "Silver";

                var updateValues = new Dictionary<string, object>
                {
                    { "phone", phone },
                    { "amountOrder", newAmount },
                    { "totalPaid", newTotalPaid },
                    { "rank", newRank }
                };
                if (name != "")
                {
                    if (foundCustomer.name == "Unknown")
                        updateValues.Add("name", name);
                }
                _dao.Customers.UpdateByQuery(updateValues, "phone_num = @phone", new Dictionary<string, object> { { "phone", phone } });

                customers.Remove(foundCustomer);
                customers.Add(new Customer(name, phone, newAmount, newTotalPaid, newRank));
                //System.Diagnostics.Debug.WriteLine(("Update new cus" + phone));

            }

            resetClick();
        }
    }
}
