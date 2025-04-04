﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class ReportData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        public ReportData(int totalOrders, decimal totalRevenue, decimal totalProfit)
        {
            TotalOrders = totalOrders;
            TotalRevenue = totalRevenue;
            TotalProfit = totalProfit;
        }

        public ReportData() { }
    }
}
