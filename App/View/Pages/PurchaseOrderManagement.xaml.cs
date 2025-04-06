using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Diagnostics;
using App.Model;
using System.Linq;
using App.View.ViewModel;
using System.Collections.Generic;
using App.Utils;
using Windows.Storage;
using System.Threading.Tasks;

namespace App.View.Pages
{
    public sealed partial class PurchaseOrderManagement : Page
    {
        public ObservableCollection<string> PresetDates { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public FullObservableCollection<Product> Products { get; set; }
        private DateTime _startDate;
        private DateTime _endDate;

        public PurchaseOrderManagement()
        {
            this.InitializeComponent();

            // Initialize preset dates for quick selection
            PresetDates = new ObservableCollection<string>()
            {
                "Hôm nay", "Hôm qua", "7 ngày qua", "Tuần trước",
                "Tháng trước", "30 ngày qua", "Trong quý", "Năm trước"
            };

            // Initialize the ProductViewModel
            ProductViewModel = new ProductViewModel();
            Products = ProductViewModel.Products;

            // Set default date range to today
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            StartDatePicker.Date = _startDate;
            EndDatePicker.Date = _endDate;
            SelectedDateText.Text = $"{_startDate:dd/MM/yyyy} - {_endDate:dd/MM/yyyy}";

            this.DataContext = this;
            ProductList.ItemsSource = Products;

            // Initialize event handlers for ComboBox changes
            CbCategory.SelectionChanged += FilterProducts;
            CbCategoryWare.SelectionChanged += FilterProducts;

            // Update the total inventory value and quantity
            UpdateTotals();

            // Ensure totals will auto-update when the list changes
            Products.CollectionChanged += (s, e) => UpdateTotals();
        }

        // Show Flyout when clicking on the date picker button
        private void DatePickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Flyout is Flyout flyout)
            {
                flyout.ShowAt(btn);
            }
        }

        // Handle preset date selection
        private void PresetDate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string selectedOption = btn.Content.ToString();
                SetDateRangeFromPreset(selectedOption);
                SelectedDateText.Text = $"{_startDate:dd/MM/yyyy} - {_endDate:dd/MM/yyyy}";

                // Apply filter with new date range
                ApplyFilters();

                // Close the flyout if it's open
                if (DatePickerFlyout.IsOpen)
                {
                    DatePickerFlyout.Hide();
                }
            }
        }

        // Set date range based on preset selection
        private void SetDateRangeFromPreset(string preset)
        {
            DateTime today = DateTime.Today;

            switch (preset)
            {
                case "Hôm nay":
                    _startDate = today;
                    _endDate = today;
                    break;
                case "Hôm qua":
                    _startDate = today.AddDays(-1);
                    _endDate = today.AddDays(-1);
                    break;
                case "7 ngày qua":
                    _startDate = today.AddDays(-6);
                    _endDate = today;
                    break;
                case "Tuần trước":
                    // Find the previous week's Monday and Sunday
                    int daysToMonday = (int)today.DayOfWeek - 1;
                    if (daysToMonday < 0) daysToMonday += 7;
                    DateTime thisMonday = today.AddDays(-daysToMonday);
                    _startDate = thisMonday.AddDays(-7);
                    _endDate = thisMonday.AddDays(-1);
                    break;
                case "Tháng trước":
                    _startDate = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                    _endDate = new DateTime(today.Year, today.Month, 1).AddDays(-1);
                    break;
                case "30 ngày qua":
                    _startDate = today.AddDays(-29);
                    _endDate = today;
                    break;
                case "Trong quý":
                    // Find the current quarter and its start/end dates
                    int currentQuarter = (today.Month - 1) / 3 + 1;
                    _startDate = new DateTime(today.Year, (currentQuarter - 1) * 3 + 1, 1);
                    _endDate = new DateTime(today.Year, currentQuarter * 3, 1).AddDays(-1);
                    break;
                case "Năm trước":
                    _startDate = new DateTime(today.Year - 1, 1, 1);
                    _endDate = new DateTime(today.Year - 1, 12, 31);
                    break;
                default:
                    _startDate = today;
                    _endDate = today;
                    break;
            }

            // Update the date pickers
            StartDatePicker.Date = _startDate;
            EndDatePicker.Date = _endDate;
        }

        // Apply custom date selection
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            _startDate = StartDatePicker.Date.Date;
            _endDate = EndDatePicker.Date.Date;

            SelectedDateText.Text = $"{_startDate:dd/MM/yyyy} - {_endDate:dd/MM/yyyy}";

            // Apply filter with new date range
            ApplyFilters();

            if (DatePickerFlyout.IsOpen)
            {
                DatePickerFlyout.Hide();
            }
        }

        // Cancel date selection
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerFlyout.IsOpen)
            {
                DatePickerFlyout.Hide();
            }
        }

        private void FilterProducts(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private async Task ApplyFilters()
        {
            try
            {
                var filter = new Dictionary<string, object>();
                var searchBox = this.FindName("TextBoxSearch") as TextBox;
                string searchText = "";

                if (searchBox != null && !string.IsNullOrEmpty(searchBox.Text))
                {
                    searchText = searchBox.Text;
                    filter.Add("Name", searchText + "%"); // Use % for partial matching
                }

                // Get selected category if not "Tất cả"
                if (CbCategory.SelectedItem is ComboBoxItem categoryItem &&
                    categoryItem.Content.ToString() != "Tất cả")
                {
                    filter.Add("TypeGroup", categoryItem.Content.ToString());
                }

                // Apply date filter if needed (requires additional implementation in the DAO)
                // filter.Add("DateFilter", new Dictionary<string, DateTime> { 
                //    { "StartDate", _startDate },
                //    { "EndDate", _endDate }
                // });

                // Default sort by name ascending
                Dictionary<string, int> sort = new Dictionary<string, int> { { "Name", 1 } };

                await ProductViewModel.NewFilter(filter, sort);
                UpdateTotals();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
                await ShowErrorDialog($"Lỗi: {ex.Message}");
            }
        }

        private async void ResetInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new EditProductDialog(Products); // Pass products list

                // Set XamlRoot if needed to avoid errors
                if (this.XamlRoot != null)
                {
                    dialog.XamlRoot = this.XamlRoot;
                }

                ContentDialogResult result = await dialog.ShowAsync();

                // After closing the dialog, update the database if changes were made
                if (result == ContentDialogResult.Primary)
                {
                    // Update database with new inventory values
                    //await UpdateProductsInventoryInDatabase();

                    // Update totals
                    UpdateTotals();
                }
            }
            catch (Exception ex)
            {
                await ShowErrorDialog($"Lỗi: {ex.Message}");
            }
        }

        private async Task UpdateProductsInventoryInDatabase()
        {
            foreach (Product product in Products)
            {
                // Create dictionary of values to update
                var updateValues = new Dictionary<string, object>
                {
                    { "Inventory", product.Inventory },
                    //{ "TotalPrice", product.Price * product.Inventory }
                };

                // Update product in database
                ProductViewModel._dao.Products.UpdateByQuery(
                    updateValues,
                    "BarCode = @BarCode",
                    new Dictionary<string, object> { { "BarCode", product.BarCode } }
                );
            }
        }

        private void UpdateTotals()
        {
            int totalQuantity = 0;
            decimal totalValue = 0;

            foreach (var product in Products)
            {
                totalQuantity += product.Inventory;
                totalValue += product.Price * product.Inventory;
            }

            TxtTotalQuantity.Text = totalQuantity.ToString("N0");
            TxtTotalValue.Text = $"{totalValue:N0} đ"; // Format with thousand separators
        }

        private async Task ShowErrorDialog(string message)
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

        private void SearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            // Check if Enter key was pressed
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ApplyFilters();
            }
        }
    }
}