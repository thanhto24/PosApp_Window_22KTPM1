using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    }
}
