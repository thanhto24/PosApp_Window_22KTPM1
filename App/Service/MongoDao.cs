using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;

namespace App.Service
{
    public class MongoDao : IDao
    {
        public IRepository<Product> Products { get; set; } = new MongoRepository<Product>();
        public IRepository<Order_> Orders { get; set; } = new MongoRepository<Order_>();
        public IRepository<Voucher> Vouchers { get; set; } = new MongoRepository<Voucher>();
        public IRepository<Category_> Categories { get; set; } = new MongoRepository<Category_>();
        public IRepository<Customer> Customers { get; set; } = new MongoRepository<Customer>();
        public IRepository<ReportData> Reports { get; set; } = new MongoRepository<ReportData>();

    }
}
