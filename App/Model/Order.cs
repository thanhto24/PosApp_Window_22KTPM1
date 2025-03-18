using System.ComponentModel;

namespace App.Model
{
    public class Order : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }
        public string InvoiceCode { get; set; }
        public string Customer { get; set; }
        public string SaleDateTime { get; set; }
        public string Salesperson { get; set; }
        public string TotalAmount { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalPayment { get; set; }
        public string TotalCost { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }

        public Order(int id, string invoiceCode, string customer, string saleDateTime, string salesperson, string totalAmount, string totalDiscount, string totalPayment, string totalCost, string paymentMethod, string status, string paymentStatus, string notes)
        {
            Id = id;
            InvoiceCode = invoiceCode;
            Customer = customer;
            SaleDateTime = saleDateTime;
            Salesperson = salesperson;
            TotalAmount = totalAmount;
            TotalDiscount = totalDiscount;
            TotalPayment = totalPayment;
            TotalCost = totalCost;
            PaymentMethod = paymentMethod;
            Status = status;
            PaymentStatus = paymentStatus;
            Notes = notes;
        }

        public Order() { }
    }
}
