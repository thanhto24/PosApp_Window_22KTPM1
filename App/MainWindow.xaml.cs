using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App.View.Pages;
using Microsoft.Data.Sqlite;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainWindow : Window
    {
        private static readonly string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");

        public MainWindow()
        {
            this.InitializeComponent();

            InitializeDatabase();

            // Điều hướng đến Login.xaml khi ứng dụng khởi động
            MainFrame.Navigate(typeof(Login));
        }

        private void InitializeDatabase()
        {
            System.Diagnostics.Debug.WriteLine($"Database Path: {DatabasePath}");

            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            CREATE TABLE IF NOT EXISTS User (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Voucher (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Code TEXT NOT NULL UNIQUE,
                StartDate TEXT NOT NULL,
                EndDate TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Note TEXT,
                MinOrder REAL NOT NULL,
                DiscountValue REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Order_ (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                InvoiceCode TEXT NOT NULL,
                Customer TEXT NOT NULL,
                SaleDateTime TEXT NOT NULL,
                Salesperson TEXT NOT NULL,
                TotalAmount REAL NOT NULL,
                TotalDiscount REAL NOT NULL,
                TotalPayment REAL NOT NULL,
                TotalCost REAL NOT NULL,
                PaymentMethod TEXT NOT NULL,
                Status TEXT NOT NULL,
                PaymentStatus TEXT NOT NULL,
                Notes TEXT
            );

            CREATE TABLE IF NOT EXISTS Product (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price TEXT NOT NULL,
                ImagePath TEXT,
                BarCode TEXT NOT NULL UNIQUE,
                TypeGroup TEXT NOT NULL,
                VAT REAL NOT NULL,
                CostPrice TEXT NOT NULL
            );
        ";
                command.ExecuteNonQuery();
            }
        }

    }
}
