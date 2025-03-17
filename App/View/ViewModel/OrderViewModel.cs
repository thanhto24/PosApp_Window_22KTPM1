using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class OrderViewModel
    {
        private IDao _dao;

        public FullObservableCollection<Order> orders { get; set; }

        public OrderViewModel() {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Order> list_order = _dao.Orders.GetAll();
            orders = new FullObservableCollection<Order>();

            foreach (Order order in list_order)
            {
                orders.Add(order);
            }
        }

        public void Add(Order item)
        {
            orders.Add(item);
        }

    }
}
