using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using App.Model;
using App.View.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using App.Utils;

namespace App.View.Pages
{
    public sealed partial class EditProductDialog : ContentDialog
    {
        public FullObservableCollection<Product> Products { get; set; }
        private ProductViewModel _productViewModel;
        private bool _isModified = false;

        private void AddCalculationHandlers()
        {
            TxtQuantity.TextChanged += UpdateTotalValue;
            TxtPrice.TextChanged += UpdateTotalValue;
        }

        public EditProductDialog()
        {
            this.InitializeComponent();
            AddCalculationHandlers(); // Add event handlers for quantity and price text changes

            Products = new FullObservableCollection<Product>(); // Initialize empty product list
            _productViewModel = new ProductViewModel();
        }

        public EditProductDialog(FullObservableCollection<Product> products) : this()
        {
            Products = products;
            CbProductCode.ItemsSource = Products; // Assign product list to ComboBox

            // If there are products, select the first one
            if (products.Count > 0)
            {
                CbProductCode.SelectedIndex = 0;
            }
        }

        // Event when "Save" button is clicked
        private async void SaveEditProduct(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (CbProductCode.SelectedItem is Product selectedProduct)
                {
                    // Update product information
                    string originalName = selectedProduct.Name;

                    // Save original inventory value to check if it changed
                    int originalInventory = selectedProduct.Inventory;

                    // Parse and validate the new inventory value
                    if (!int.TryParse(TxtQuantity.Text, out int newInventory) || newInventory < 0)
                    {
                        args.Cancel = true; // Prevent dialog from closing
                        await ShowErrorMessage("Số lượng không hợp lệ. Vui lòng nhập số không âm.");
                        return;
                    }

                    // Parse and validate the price
                    if (!int.TryParse(TxtPrice.Text, out int newPrice) || newPrice < 0)
                    {
                        args.Cancel = true; // Prevent dialog from closing
                        await ShowErrorMessage("Giá không hợp lệ. Vui lòng nhập số không âm.");
                        return;
                    }

                    // Update the product properties
                    selectedProduct.Name = TxtProductName.Text;
                    selectedProduct.Inventory = newInventory;
                    selectedProduct.Price = newPrice;
                    selectedProduct.TotalPrice = newInventory * newPrice;

                    // Update UI and mark as modified if inventory changed
                    if (originalInventory != newInventory)
                    {
                        _isModified = true;
                    }

                    // Update the product in the database
                    var updateValues = new Dictionary<string, object>
                    {
                        { "Name", selectedProduct.Name },
                        { "Inventory", selectedProduct.Inventory },
                        { "Price", selectedProduct.Price },
                        { "TotalPrice", selectedProduct.TotalPrice }
                    };

                    // Update in database using repository
                    _productViewModel._dao.Products.UpdateByQuery(
                        updateValues,
                        "BarCode = @BarCode",
                        new Dictionary<string, object> { { "BarCode", selectedProduct.BarCode } }
                    );

                    Debug.WriteLine($"Updated product: {selectedProduct.Name}, Inventory: {selectedProduct.Inventory}");
                }
            }
            catch (Exception ex)
            {
                args.Cancel = true; // Prevent dialog from closing
                await ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        // Event when "Cancel" button is clicked
        private void CancelEditProduct(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // No additional action needed, the dialog will be dismissed
        }

        // Show error message
        private async Task ShowErrorMessage(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await errorDialog.ShowAsync();
        }

        // Event when a product is selected from the dropdown
        private void ProductSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbProductCode.SelectedItem is Product selectedProduct)
            {
                TxtProductName.Text = selectedProduct.Name;
                TxtQuantity.Text = selectedProduct.Inventory.ToString();
                TxtPrice.Text = selectedProduct.Price.ToString();

                // Update total value
                UpdateTotalValue(null, null);
            }
        }

        // Check if any changes were made
        public bool IsModified()
        {
            return _isModified;
        }


        

        // Add this method to update the total value when quantity or price changes
        private void UpdateTotalValue(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtQuantity.Text, out int quantity) &&
                int.TryParse(TxtPrice.Text, out int price))
            {
                int totalValue = quantity * price;
                TxtTotalValue.Text = $"{totalValue:N0} đ";
            }
            else
            {
                TxtTotalValue.Text = "0 đ";
            }
        }
    }
}