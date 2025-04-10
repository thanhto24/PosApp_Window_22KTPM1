using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using App.Model;

namespace App.View.ViewModel
{
    public class LineChartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ISeries[] _revenueSeries;
        public ISeries[] RevenueSeries
        {
            get => _revenueSeries;
            set
            {
                _revenueSeries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RevenueSeries)));
            }
        }

        private Axis[] _xAxes;
        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XAxes)));
            }
        }

        private Axis[] _yAxes;
        public Axis[] YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YAxes)));
            }
        }

        public LineChartViewModel()
        {
            InitializeAxes();
        }

        private void InitializeAxes()
        {
            XAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Thời gian",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 10
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Doanh thu (VNĐ)",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 10,
                    Labeler = value => string.Format(CultureInfo.CurrentCulture, "{0:N0}đ", value)
                }
            };
        }

        public void UpdateChart(List<Order_> orders, string timeFrame)
        {
            if (orders == null || orders.Count == 0)
            {
                // If no orders, show empty chart
                RevenueSeries = new ISeries[]
                {
                    new LineSeries<decimal>
                    {
                        Values = new List<decimal>(),
                        Fill = null,
                        GeometryFill = new SolidColorPaint(SKColors.Blue),
                        GeometrySize = 8,
                        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                        Name = "Doanh thu"
                    },
                    new LineSeries<decimal>
                    {
                        Values = new List<decimal>(),
                        Fill = null,
                        GeometryFill = new SolidColorPaint(SKColors.Green),
                        GeometrySize = 8,
                        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
                        Name = "Lợi nhuận"
                    }
                };
                return;
            }

            var revenueData = new Dictionary<string, decimal>();
            var profitData = new Dictionary<string, decimal>();

            switch (timeFrame)
            {
                case "day":
                    ProcessDailyData(orders, revenueData, profitData);
                    break;
                case "month":
                    ProcessMonthlyData(orders, revenueData, profitData);
                    break;
                case "year":
                    ProcessYearlyData(orders, revenueData, profitData);
                    break;
            }

            UpdateChartSeries(revenueData, profitData);
        }

        private void ProcessDailyData(List<Order_> orders, Dictionary<string, decimal> revenueData, Dictionary<string, decimal> profitData)
        {
            // Group by day - last 14 days
            var startDate = DateTime.Now.AddDays(-14);

            // Create all dates in range
            for (var date = startDate; date <= DateTime.Now; date = date.AddDays(1))
            {
                string key = date.ToString("dd/MM");
                revenueData[key] = 0;
                profitData[key] = 0;
            }

            // Fill with actual data
            foreach (var order in orders)
            {
                //DateTime orderDate = order.SaleDateTime.ToLocalTime();
                DateTime orderDate = DateTime.Parse(order.SaleDateTime).ToLocalTime();

                if (orderDate >= startDate)
                {
                    string key = orderDate.ToString("dd/MM");
                    if (revenueData.ContainsKey(key))
                    {
                        revenueData[key] += order.TotalPayment;
                        profitData[key] += (order.TotalAmount - order.TotalCost);
                    }
                    else
                    {
                        revenueData[key] = order.TotalPayment;
                        profitData[key] = (order.TotalAmount - order.TotalCost);
                    }
                }
            }
        }

        private void ProcessMonthlyData(List<Order_> orders, Dictionary<string, decimal> revenueData, Dictionary<string, decimal> profitData)
        {
            // Group by month - last 12 months
            var startDate = DateTime.Now.AddMonths(-11);

            // Create all months in range
            for (var date = startDate; date <= DateTime.Now; date = date.AddMonths(1))
            {
                string key = date.ToString("MM/yyyy");
                revenueData[key] = 0;
                profitData[key] = 0;
            }

            // Fill with actual data
            foreach (var order in orders)
            {
                //DateTime orderDate = order.SaleDateTime.ToLocalTime();
                DateTime orderDate = DateTime.Parse(order.SaleDateTime).ToLocalTime();

                if (orderDate >= startDate)
                {
                    string key = orderDate.ToString("MM/yyyy");
                    if (revenueData.ContainsKey(key))
                    {
                        revenueData[key] += order.TotalPayment;
                        profitData[key] += (order.TotalAmount - order.TotalCost);
                    }
                    else
                    {
                        revenueData[key] = order.TotalPayment;
                        profitData[key] = (order.TotalAmount - order.TotalCost);
                    }   
                }
            }
        }

        private void ProcessYearlyData(List<Order_> orders, Dictionary<string, decimal> revenueData, Dictionary<string, decimal> profitData)
        {
            // Group by year - last 5 years
            var startYear = DateTime.Now.Year - 4;

            // Create all years in range
            for (int year = startYear; year <= DateTime.Now.Year; year++)
            {
                string key = year.ToString();
                revenueData[key] = 0;
                profitData[key] = 0;
            }

            // Fill with actual data
            foreach (var order in orders)
            {
                //DateTime orderDate = order.SaleDateTime.ToLocalTime();
                DateTime orderDate = DateTime.Parse(order.SaleDateTime).ToLocalTime();

                if (orderDate.Year >= startYear)
                {
                    string key = orderDate.Year.ToString();
                    if (revenueData.ContainsKey(key))
                    {
                        revenueData[key] += order.TotalPayment;
                        profitData[key] += (order.TotalAmount - order.TotalCost);
                    }
                    else
                    {
                        revenueData[key] = order.TotalPayment;
                        profitData[key] = (order.TotalAmount - order.TotalCost);
                    }
                }
            }
        }

        private void UpdateChartSeries(Dictionary<string, decimal> revenueData, Dictionary<string, decimal> profitData)
        {
            var keys = revenueData.Keys.ToArray();
            XAxes[0].Labels = keys;

            RevenueSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = keys.Select(k => revenueData[k]).ToList(),
                    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(40)),
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                    Name = "Doanh thu"
                },
                new LineSeries<decimal>
                {
                    Values = keys.Select(k => profitData[k]).ToList(),
                    Fill = new SolidColorPaint(SKColors.Green.WithAlpha(40)),
                    GeometryFill = new SolidColorPaint(SKColors.Green),
                    GeometrySize = 8,
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
                    Name = "Lợi nhuận"
                }
            };
        }
    }
}