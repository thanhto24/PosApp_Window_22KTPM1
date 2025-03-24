using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using App.Model;
using System.Linq;
using System.Collections.Generic;

namespace App.View.Pages
{
    public sealed partial class HomeScreen : Page
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> OrderedProducts { get; set; } = new ObservableCollection<Product>();
        private Dictionary<string, List<Product>> categoryProducts = new();
        private string currentCategory = "Cà phê";

        public HomeScreen()
        {
            this.InitializeComponent();
            this.DataContext = this;
            LoadCoffeeProducts();
        }

        private void DrinkCategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrinkCategoryList.SelectedItem is not ListViewItem selectedItem)
                return;

            string category = selectedItem.Content?.ToString() ?? "Unknown";
            Debug.WriteLine($"Chọn danh mục: {category}");

            SaveProductQuantities(currentCategory);
            currentCategory = category;
            RestoreProductQuantities(category);
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                product.Quantity++;
                UpdateOrderList(product);
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Product product)
            {
                if (product.Quantity > 0)
                {
                    product.Quantity--;
                    UpdateOrderList(product);
                }
            }
        }

        private void UpdateOrderList(Product product)
        {
            var existingProduct = OrderedProducts.FirstOrDefault(p => p.ProductCode == product.ProductCode);

            if (product.Quantity > 0)
            {
                if (existingProduct == null)
                {
                    OrderedProducts.Add(product);
                }
                else
                {
                    existingProduct.Quantity = product.Quantity;
                }
            }
            else
            {
                if (existingProduct != null)
                {
                    OrderedProducts.Remove(existingProduct);
                }
            }
            UpdateOrderSummary();
        }


        private void UpdateOrderSummary()
        {
            int totalItems = OrderedProducts.Sum(p => p.Quantity);
            OrderSummaryText.Text = $"Các món order ({totalItems} món)";
        }

        private void SaveProductQuantities(string category)
        {
            if (!categoryProducts.ContainsKey(category))
            {
                categoryProducts[category] = new List<Product>();
            }

            categoryProducts[category] = Products.Select(p =>
                new Product(p.ProductCode, p.Name, p.Quantity, p.Price, p.ImagePath)).ToList();
        }

        private void RestoreProductQuantities(string category)
        {
            Products.Clear();
            if (categoryProducts.TryGetValue(category, out var savedProducts))
            {
                foreach (var product in savedProducts)
                {
                    Products.Add(new Product(product.ProductCode, product.Name, product.Quantity, product.Price, product.ImagePath));
                }
            }
            else
            {
                LoadProductsByCategory(category);
            }
        }

        private void LoadProductsByCategory(string category)
        {
            switch (category)
            {
                case "Cà phê":
                    LoadCoffeeProducts();
                    break;
                case "Sinh tố":
                    LoadSmoothieProducts();
                    break;
                case "Nước ép":
                    LoadJuiceProducts();
                    break;
                case "Trà sữa":
                    LoadMilkTeaProducts();
                    break;
                case "Đồ ăn vặt":
                    LoadSnackProducts();
                    break;
                default:
                    Products.Clear();
                    break;
            }
        }

        private void LoadCoffeeProducts()
        {
            Products.Clear();
            Products.Add(new Product("CF001", "Espresso", 0, 32000, "ms-appx:///Assets/espresso.jpg"));
            Products.Add(new Product("CF002", "Cappuccino", 0, 35000, "ms-appx:///Assets/cappuccino.jpg"));
            Products.Add(new Product("CF003", "Latte", 0, 40000, "ms-appx:///Assets/latte.jpg"));
        }

        private void LoadSmoothieProducts()
        {
            Products.Clear();
            Products.Add(new Product("ST001", "Sinh tố bơ", 0, 45000, "ms-appx:///Assets/smoothie_avocado.jpg"));
            Products.Add(new Product("ST002", "Sinh tố xoài", 0, 40000, "ms-appx:///Assets/smoothie_mango.jpg"));
            Products.Add(new Product("ST003", "Sinh tố dâu", 0, 42000, "ms-appx:///Assets/smoothie_strawberry.jpg"));
        }

        private void LoadJuiceProducts()
        {
            Products.Clear();
            Products.Add(new Product("NJ001", "Ép cam", 0, 35000, "ms-appx:///Assets/juice_orange.jpg"));
            Products.Add(new Product("NJ002", "Ép dưa hấu", 0, 30000, "ms-appx:///Assets/juice_watermelon.jpg"));
            Products.Add(new Product("NJ003", "Ép cà rốt", 0, 32000, "ms-appx:///Assets/juice_carrot.jpg"));
        }

        private void LoadMilkTeaProducts()
        {
            Products.Clear();
            Products.Add(new Product("TS001", "Trà sữa\ntruyền thống", 0, 38000, "ms-appx:///Assets/milktea_classic.jpg"));
            Products.Add(new Product("TS002", "Trà sữa\ntrân châu đen", 0, 40000, "ms-appx:///Assets/milktea_blackpearl.jpg"));
            Products.Add(new Product("TS003", "Trà sữa\nmatcha", 0, 42000, "ms-appx:///Assets/milktea_matcha.jpg"));
        }

        private void LoadSnackProducts()
        {
            Products.Clear();
            Products.Add(new Product("SN001", "Khoai tây chiên", 0, 30000, "ms-appx:///Assets/snack_fries.jpg"));
            Products.Add(new Product("SN002", "Gà rán", 0, 45000, "ms-appx:///Assets/snack_friedchicken.jpg"));
            Products.Add(new Product("SN003", "Bánh mì sandwich", 0, 35000, "ms-appx:///Assets/snack_sandwich.jpg"));
        }
    }
}
