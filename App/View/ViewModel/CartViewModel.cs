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
        private Dictionary<string, int> cart;  // Khởi tạo từ điển lưu số lượng sản phẩm

        public CartViewModel()
        {
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
            double total = CartItems.Sum(item => int.TryParse(item.Product.CostPrice.ToString(), out int price) ? price * item.Quantity : 0);
            return total;
        }

        public void Clear_()
        {
            CartItems.Clear();
            cart.Clear();
        }
    }
}
