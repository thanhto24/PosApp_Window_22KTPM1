using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;
using App.Service;
using App.Utils;
using Newtonsoft.Json;


namespace App.View.ViewModel
{
    public class CartViewModel
    {
        public FullObservableCollection<CartItem> CartItems { get; set; }
        private Dictionary<string, int> cart;
        private IDao _dao;
        public double totalDiscount;
        public bool IsVATEnabled { get; set; } = false;
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
                    // Check if adding one more would exceed inventory
                    if (existingItem.Quantity + 1 > product.Inventory)
                    {
                        // This should not happen as we check in the UI, but just in case
                        return;
                    }

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

                        if (existingItem.Quantity <= 0)  
                        {
                            CartItems.Remove(existingItem);
                        }
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
            double total = CartItems.Sum(item =>
            {
                double vatMultiplier = IsVATEnabled ? (1 + item.Product.Vat / 100.0) : 1.0;
                return item.Product.Price * item.Quantity * vatMultiplier;
            });

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
            List<OrderedProduct> orderedProductsList = CartItems.Select(ci => new OrderedProduct("PC01", ci.Product.Name, ci.Quantity, (decimal)ci.Product.Price)).ToList();
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

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(order, Formatting.Indented);
            Console.WriteLine(json);

            _dao.Orders.Insert(order);
        }


    }
}
