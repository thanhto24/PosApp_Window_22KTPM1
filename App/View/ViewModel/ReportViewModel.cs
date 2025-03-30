using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;
using App.Service;
using App.Utils;
using Microsoft.UI.Xaml.Controls;

namespace App.View.ViewModel
{
    public class ReportViewModel
    {
        private IDao _dao;

        public FullObservableCollection<ReportData> report { get; set; }

        public ReportViewModel()
        {
            _dao = Services.GetKeyedSingleton<IDao>();

            report = new FullObservableCollection<ReportData>();

        }

        public List<Order_> GetAllOrders()
        {
            return _dao.Orders.GetAll();
        }

        public void addReport(ReportData reportData)
        {
            _dao.Reports.Insert(reportData);
        }
    }

}
