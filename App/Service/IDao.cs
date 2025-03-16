using App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Service
{
    public interface IDao
    {
        public IRepository<Product> Categories { get; set; }
        public IRepository<Order> Orders { get; set; }
    }
}
