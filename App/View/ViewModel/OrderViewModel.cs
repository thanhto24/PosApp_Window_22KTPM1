using System.Collections.Generic;
using System.Linq;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class OrderViewModel
    {
        private IDao _dao;
        private const int ItemsPerPage = 5; // Số lượng đơn hàng trên mỗi trang

        public FullObservableCollection<Order_> orders { get; set; }
        public FullObservableCollection<Order_> DisplayedOrders { get; set; }

        public int CurrentPage { get; private set; } = 1;
        public int TotalPages => (int)System.Math.Ceiling((double)orders.Count / ItemsPerPage);

        public OrderViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Order_> list_order = _dao.Orders.GetAll();
            orders = new FullObservableCollection<Order_>(list_order);
            DisplayedOrders = new FullObservableCollection<Order_>();

            UpdateDisplayedOrders();
        }

        private void UpdateDisplayedOrders()
        {
            DisplayedOrders.Clear();
            foreach (var order in orders.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage))
            {
                DisplayedOrders.Add(order);
            }
        }

        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                UpdateDisplayedOrders();
            }
        }

        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdateDisplayedOrders();
            }
        }

        public void Add(Order_ item)
        {
            orders.Add(item);
            UpdateDisplayedOrders(); // Cập nhật danh sách hiển thị sau khi thêm
        }
    }
}
