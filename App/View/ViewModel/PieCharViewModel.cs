using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using App.Model;
using App.Service;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace App.View.ViewModel
{
    public class PieChartViewModel
    {
        private ReportViewModel _reportViewModel;

        public PieChartViewModel()
        {
            _reportViewModel = new ReportViewModel();
            ProductSalesData = new ObservableCollection<ProductSalesData>();
            PieSeries = new ObservableCollection<ISeries>();
            LoadData();
        }

        public ObservableCollection<ProductSalesData> ProductSalesData { get; set; }
        public ObservableCollection<ISeries> PieSeries { get; set; }

        public void LoadData()
        {
            // Get all orders from the database
            var orders = _reportViewModel.GetAllOrders();

            // Dictionary to track product sales
            Dictionary<string, ProductSalesData> productSales = new Dictionary<string, ProductSalesData>();

            // Process all orders to extract product sales data
            foreach (var order in orders)
            {
                if (order.OrderedProducts != null)
                {
                    foreach (var product in order.OrderedProducts)
                    {
                        string productName = product.name;

                        if (!productSales.ContainsKey(productName))
                        {
                            productSales[productName] = new ProductSalesData
                            {
                                ProductName = productName,
                                ProductCode = product.productCode,
                                Quantity = 0,
                                TotalRevenue = 0
                            };
                        }

                        productSales[productName].Quantity += product.quantity;
                        productSales[productName].TotalRevenue += product.price * product.quantity;
                    }
                }
            }

            // Convert to list and sort by quantity
            var sortedProducts = productSales.Values.OrderByDescending(p => p.Quantity).ToList();

            // Clear existing collections
            ProductSalesData.Clear();
            PieSeries.Clear();

            // Add sorted data to the observable collection
            foreach (var product in sortedProducts)
            {
                ProductSalesData.Add(product);
            }

            // Create pie chart data
            // If there are many products, combine the smallest ones into "Others"
            int maxSlices = 5;

            // Generate colors for the chart
            var colors = new List<SKColor>
            {
                new SKColor(220, 20, 60),    // Crimson
                new SKColor(30, 144, 255),   // DodgerBlue
                new SKColor(34, 139, 34),    // ForestGreen
                new SKColor(255, 165, 0),    // Orange
                new SKColor(128, 0, 128)     // Purple
            };

            if (sortedProducts.Count <= maxSlices)
            {
                // Show all products if there are few
                for (int i = 0; i < sortedProducts.Count; i++)
                {
                    var product = sortedProducts[i];
                    var color = i < colors.Count ? colors[i] : new SKColor(128, 128, 128); // Use gray for additional items

                    PieSeries.Add(new PieSeries<int>
                    {
                        Values = new[] { product.Quantity },
                        Name = product.ProductName,
                        Fill = new SolidColorPaint(color),
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{product.ProductName}: {point.PrimaryValue}"
                    });
                }
            }
            else
            {
                // Show top products and combine the rest
                for (int i = 0; i < maxSlices - 1; i++)
                {
                    var product = sortedProducts[i];
                    var color = colors[i];

                    PieSeries.Add(new PieSeries<int>
                    {
                        Values = new[] { product.Quantity },
                        Name = product.ProductName,
                        Fill = new SolidColorPaint(color),
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                        DataLabelsFormatter = point => $"{product.ProductName}: {point.PrimaryValue}"
                    });
                }

                // Combine remaining products as "Others"
                int othersQuantity = sortedProducts.Skip(maxSlices - 1).Sum(p => p.Quantity);

                PieSeries.Add(new PieSeries<int>
                {
                    Values = new[] { othersQuantity },
                    Name = "Khác",
                    Fill = new SolidColorPaint(colors[maxSlices - 1]),
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"Khác: {point.PrimaryValue}"
                });
            }
        }
    }

    public class ProductSalesData
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}