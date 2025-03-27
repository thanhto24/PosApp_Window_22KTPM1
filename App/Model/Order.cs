using System.Collections.Generic;
using System.ComponentModel;

namespace App.Model
{
    public class Order_ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }
        public string InvoiceCode { get; set; }
        public string Customer { get; set; }
        public string SaleDateTime { get; set; }
        private List<Product> orderedProducts { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalCost { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }

        public Order_(int id, string invoiceCode, string customer, string saleDateTime, List<Product> orderedProducts, decimal totalAmount, decimal totalDiscount, decimal totalPayment, decimal totalCost, string paymentMethod, string status, string paymentStatus, string notes)
        {
            Id = id;
            InvoiceCode = invoiceCode;
            Customer = customer;
            SaleDateTime = saleDateTime;
            orderedProducts = orderedProducts;
            TotalAmount = totalAmount;
            TotalDiscount = totalDiscount;
            TotalPayment = totalPayment;
            TotalCost = totalCost;
            PaymentMethod = paymentMethod;
            Status = status;
            PaymentStatus = paymentStatus;
            Notes = notes;
        }

        public Order_() { }
    }
}