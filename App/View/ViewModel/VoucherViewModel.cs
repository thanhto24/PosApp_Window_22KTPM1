﻿using System;
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

        public bool isDup(string code)
        {
            foreach (Voucher voucher in vouchers)
                if (code.Equals(voucher.Code))
                    return true;
            return false;
        }

        public void Add(Voucher item)
        {
            vouchers.Add(item);
        }

        public void CreateVoucher(string code, DateTimeOffset? startDate, DateTimeOffset? endDate, int quantity, decimal minOrder, decimal discountValue, string note)
        {
            var newVoucher = new Voucher(code, startDate, endDate, quantity, minOrder, discountValue, note);
            _dao.Vouchers.Insert(newVoucher); // Chèn vào database
            Add(newVoucher); // Cập nhật UI
            return;
        }

        public void RemoveVoucher(string code)
        {
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


            if (oldVoucher == null)
            {
                System.Diagnostics.Debug.WriteLine("Khong ton tai vc");
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
            System.Diagnostics.Debug.WriteLine("Update vc thanh cong");

        }

        //public double ApplyVoucher(string code)
        //{
        //    var filter = new Dictionary<string, object>
        //    {
        //        {"Code", code },
        //    };
        //    var voucher = _dao.Vouchers.GetByQuery(filter);
        //    if (voucher != null && voucher.Any())
        //    {
        //        return (double)voucher[0].DiscountValue;
        //    }
        //    return 0;
        //}
        public double ApplyVoucher(string code, double orderValue)
        {
            var filter = new Dictionary<string, object>
    {
        {"Code", code },
    };
            var voucher = _dao.Vouchers.GetByQuery(filter);

            if (voucher != null && voucher.Any())
            {
                var currentVoucher = voucher[0];
                DateTime currentDate = DateTime.Now;

                // Kiểm tra thời gian hiệu lực
                if (currentDate < currentVoucher.StartDate || currentDate > currentVoucher.EndDate)
                {
                    // Voucher hết hạn hoặc chưa có hiệu lực
                    return 0;
                }

                // Kiểm tra giá trị đơn hàng tối thiểu
                if (orderValue < (double)currentVoucher.MinOrder)
                {
                    // Đơn hàng không đạt giá trị tối thiểu
                    return 0;
                }

                // Voucher hợp lệ, trả về giá trị giảm giá
                return (double)currentVoucher.DiscountValue;
            }

            // Không tìm thấy voucher
            return 0;
        }

        public void des(string code)
        {
            var filter = new Dictionary<string, object>
            {
                {"Code", code },
            };
            var voucher = _dao.Vouchers.GetByQuery(filter);
            if (voucher != null && voucher.Any())
            {
                Voucher old_vc = voucher[0];
                int new_amount = old_vc.Quantity - 1;
                Voucher new_vc = voucher[0];
                new_vc.Quantity = new_amount;
                if (new_amount > 0)
                {
                    var updateValues = new Dictionary<string, object>
                    {
                        { "Quantity", new_amount },
                    };
                    _dao.Vouchers.UpdateByQuery(updateValues,
                                                "Code = @code",
                                                new Dictionary<string, object> { { "code", code } });
                    vouchers.Remove(old_vc);
                    vouchers.Add(new_vc);
                }
                else
                {
                    _dao.Vouchers.RemoveByQuery("Code = @code",
                                                new Dictionary<string, object> { { "code", code } });
                    vouchers.Remove(old_vc);
                }
            }
        }
    }
}
