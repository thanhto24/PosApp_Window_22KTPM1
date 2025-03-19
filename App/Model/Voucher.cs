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
        public int Quantity { get; set; } // Số lượng voucher
        public string Note { get; set; } // Ghi chú (nếu có)
        public decimal MinOrder { get; set; } // Giá trị đơn hàng tối thiểu
        public decimal DiscountValue { get; set; } // Giá trị giảm giá
        public DateTimeOffset? StartDate { get; set; } // Dùng nullable để tránh lỗi
        public DateTimeOffset? EndDate { get; set; } // Dùng nullable để tránh lỗi

        public Voucher() { }

        public Voucher(string code, DateTimeOffset? startDate, DateTimeOffset? endDate, int quantity, decimal minOrder, decimal discountValue, string note)
        {
            Code = code;
            StartDate = startDate;
            EndDate = endDate;
            Quantity = quantity;
            MinOrder = minOrder;
            DiscountValue = discountValue;
            Note = note;
        }
    }
}
