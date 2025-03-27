using App.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace App.Model
{
    public class Order_ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        private string _invoiceCode;
        private string _customer;
        private string _saleDateTime;
        private List<Product> _orderedProducts;
        private decimal _totalAmount;
        private decimal _totalDiscount;
        private decimal _totalPayment;
        private decimal _totalCost;
        private string _paymentMethod;
        private string _status;
        private string _paymentStatus;
        private string _notes;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string InvoiceCode
        {
            get => _invoiceCode;
            set { _invoiceCode = value; OnPropertyChanged(nameof(InvoiceCode)); }
        }

        public string Customer
        {
            get => _customer;
            set { _customer = value; OnPropertyChanged(nameof(Customer)); }
        }

        public string SaleDateTime
        {
            get => _saleDateTime;
            set { _saleDateTime = value; OnPropertyChanged(nameof(SaleDateTime)); }
        }

        public List<Product> OrderedProducts
        {
            get => _orderedProducts;
            set { _orderedProducts = value; OnPropertyChanged(nameof(OrderedProducts)); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(nameof(TotalAmount)); }
        }

        public decimal TotalDiscount
        {
            get => _totalDiscount;
            set { _totalDiscount = value; OnPropertyChanged(nameof(TotalDiscount)); }
        }

        public decimal TotalPayment
        {
            get => _totalPayment;
            set { _totalPayment = value; OnPropertyChanged(nameof(TotalPayment)); }
        }

        public decimal TotalCost
        {
            get => _totalCost;
            set { _totalCost = value; OnPropertyChanged(nameof(TotalCost)); }
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(nameof(PaymentMethod)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string PaymentStatus
        {
            get => _paymentStatus;
            set { _paymentStatus = value; OnPropertyChanged(nameof(PaymentStatus)); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }

        public Order_() { }

        public Order_(int id, string invoiceCode, string customer, string saleDateTime, List<Product> orderedProducts,
            decimal totalAmount, decimal totalDiscount, decimal totalPayment, decimal totalCost,
            string paymentMethod, string status, string paymentStatus, string notes)
        {
            _id = id;
            _invoiceCode = invoiceCode;
            _customer = customer;
            _saleDateTime = saleDateTime;
            _orderedProducts = orderedProducts;
            _totalAmount = totalAmount;
            _totalDiscount = totalDiscount;
            _totalPayment = totalPayment;
            _totalCost = totalCost;
            _paymentMethod = paymentMethod;
            _status = status;
            _paymentStatus = paymentStatus;
            _notes = notes;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
