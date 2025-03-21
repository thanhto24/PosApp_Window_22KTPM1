using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Model;
using Microsoft.Data.Sqlite;

namespace App.Service
{
    public class SQLDao : IDao
    {
        
        public IRepository<Product> Categories { get; set; } = new SQLRepository<Product>();
        public IRepository<Order_> Orders { get; set; } = new SQLRepository<Order_>();
        public IRepository<Voucher> Vouchers { get; set; } = new SQLRepository<Voucher>();

    }
}
