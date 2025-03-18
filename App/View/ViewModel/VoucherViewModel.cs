using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using App.Model;
using App.Service;
using App.Utils;

namespace App.View.ViewModel
{
    public class VoucherViewModel 
    {
        private IDao _dao;

        public FullObservableCollection<Voucher> vouchers { get; set; }

        public VoucherViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            List<Voucher> list_Voucher = _dao.Vouchers.GetAll();
            vouchers = new FullObservableCollection<Voucher>();

            foreach (Voucher voucher in list_Voucher)
            {
                vouchers.Add(voucher);
            }
        }

        public void Add(Voucher item)
        {
            vouchers.Add(item);
        }

        public void CreateVoucher(string code, DateTimeOffset? startDate, DateTimeOffset? endDate, int quantity, decimal minOrder, decimal discountValue, string note)
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            var newVoucher = new Voucher(code, startDate, endDate, quantity, minOrder, discountValue, note);
            _dao.Vouchers.Insert(newVoucher); // Chèn vào database
            Add(newVoucher); // Cập nhật UI
            return;
        }

        public void RemoveVoucher(string code)
        {
            _dao = Services.GetKeyedSingleton<IDao>();
            _dao.Vouchers.RemoveByQuery("Code = @code", new Dictionary<string, object>
            {
                { "code", code }
            });
            var voucherToRemove = vouchers.FirstOrDefault(v => v.Code == code);
            if (voucherToRemove != null)
            {
                vouchers.Remove(voucherToRemove);
            }
        }

        public void UpdateVoucher(Voucher oldVoucher, string code, DateTimeOffset? startDate, DateTimeOffset? endDate, int quantity, decimal minOrder, decimal discountValue, string note)
        {
            
            Voucher item = new Voucher(code, startDate, endDate, quantity, minOrder, discountValue, note);

            _dao = Services.GetKeyedSingleton<IDao>();


            if (oldVoucher == null)
            {
                throw new Exception("Voucher không tồn tại.");
            }

            // Tạo Dictionary chứa dữ liệu cần cập nhật
            var updateValues = new Dictionary<string, object>
            {
                { "Code", item.Code },
                { "Quantity", item.Quantity },
                { "Note", item.Note },
                { "MinOrder", item.MinOrder },
                { "DiscountValue", item.DiscountValue }
            };

            // Xử lý StartDate và EndDate mà không dùng ??
            if (item.StartDate.HasValue)
            {
                updateValues["StartDate"] = item.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                updateValues["StartDate"] = DBNull.Value;
            }

            if (item.EndDate.HasValue)
            {
                updateValues["EndDate"] = item.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                updateValues["EndDate"] = DBNull.Value;
            }

            // Cập nhật database bằng UpdateByQuery
            _dao.Vouchers.UpdateByQuery(
                updateValues,
                "Code = @code",
                new Dictionary<string, object> { { "code", oldVoucher.Code } }
            );

            // Cập nhật danh sách vouchers
            vouchers.Remove(oldVoucher);
            vouchers.Add(item);
        }

    }
}
