using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Model
{
    public class Voucher : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Code { get; set; } // Mã giảm giá
        public DateTime StartDate { get; set; } // Ngày bắt đầu hiệu lực
        public DateTime EndDate { get; set; } // Ngày kết thúc hiệu lực
        public int Quantity { get; set; } // Số lượng voucher
        public string Note { get; set; } // Ghi chú (nếu có)
        public decimal MinOrderValue { get; set; } // Giá trị đơn hàng tối thiểu
        public decimal DiscountValue { get; set; } // Giá trị giảm (số tiền hoặc % giảm)
        public decimal MaxDiscount { get; set; } // Mức giảm tối đa
        public string FormattedDate => $"HSD: {StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
        public string FormattedDiscount => $"{DiscountValue:N0} VNĐ";
        public string FormattedMinOrderValue => $"{MinOrderValue:N0} VNĐ";


        public Voucher(string code, DateTime startDate, DateTime endDate, int quantity, string note, decimal minOrderValue, decimal discountValue, decimal maxDiscount)
        {
            Code = code;
            StartDate = startDate;
            EndDate = endDate;
            Quantity = quantity;
            Note = note;
            MinOrderValue = minOrderValue;
            DiscountValue = discountValue;
            MaxDiscount = maxDiscount;
        }
    }
}
