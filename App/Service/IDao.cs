﻿using App.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Service
{
    public interface IDao
    {
        public IRepository<Product> Categories { get; set; }
        public IRepository<Order_> Orders { get; set; }
        public IRepository<Voucher> Vouchers { get; set; }
    }
}
