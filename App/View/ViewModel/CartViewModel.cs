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
    public class CartViewModel
    {
        public FullObservableCollection<CartItem> CartItems { get; set; }
        private Dictionary<string, int> cart;
        private IDao _dao;
        public double totalDiscount;
        public CartViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            CartItems = new FullObservableCollection<CartItem>();
            cart = new Dictionary<string, int>();
        }

        public void AddToCart(Product product)
        {
            if (cart.ContainsKey(product.Name))
            {
                cart[product.Name]++;
                var existingItem = CartItems.FirstOrDefault(ci => ci.Product.Name == product.Name);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
            }
            else
            {
                cart[product.Name] = 1;
                CartItems.Add(new CartItem { Product = product, Quantity = 1 });
            }
        }

        public void RemoveFromCart(Product product)
        {
            if (cart.ContainsKey(product.Name))
            {
                if (cart[product.Name] > 1)
                {
                    cart[product.Name]--;
                    var existingItem = CartItems.FirstOrDefault(ci => ci.Product.Name == product.Name);
                    if (existingItem != null)
                    {
                        existingItem.Quantity--;
                    }
                }
                else
                {
                    cart.Remove(product.Name);
                    var itemToRemove = CartItems.FirstOrDefault(ci => ci.Product.Name == product.Name);
                    if (itemToRemove != null)
                    {
                        CartItems.Remove(itemToRemove);
                    }
                }
            }
        }

        public double getTotalAmount()
        {
            double total = CartItems.Sum(item => item.Product.Price * item.Quantity);
            return total;
        }

        public int getTotalQuantity()
        {
            int total = CartItems.Sum(item => item.Quantity);
            return total;
        }

        public void Clear_()
        {
            CartItems.Clear();
            cart.Clear();
        }


        public void CreateNewOrder(double totalDiscount, string nameCustomer)
        {
            int newId = _dao.Orders.GetAll().Count + 1;
            string invoiceId = $"INV{newId:D3}";

            string customerName = nameCustomer;
            List<Product> orderedProductsList = CartItems.Select(ci => ci.Product).ToList();
            decimal totalAmount = (decimal)getTotalAmount();
            decimal discount = (decimal)totalDiscount;
            decimal finalAmount = totalAmount - discount;
            decimal totalCost = CartItems.Sum(p => (decimal)p.Product.CostPrice * p.Quantity);


            var order = new Order_(
                id: newId,
                invoiceCode: invoiceId,
                customer: customerName,
                saleDateTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                orderedProducts: orderedProductsList,
                totalAmount: totalAmount,
                totalDiscount: discount,
                totalPayment: finalAmount,
                totalCost: totalCost,
                paymentMethod: "Tiền mặt",
                status: "Đã giao",
                paymentStatus: "Đã thanh toán",
                notes: "Giao hàng thành công"
            );

            _dao.Orders.Insert(order);
        }
    }
}
