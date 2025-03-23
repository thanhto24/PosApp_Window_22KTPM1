using App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Service
{
    public class MockDao : IDao
    {
        public class MockCategoryRepository : IRepository<Product>
        {
            public List<Product> GetAll()
            {
                        return new List<Product>() {
                            new Product ("SP001", "Hồng Trà Đài Loan", 0, 12000, "ms-appx:///Assets/tea1.jpg"),
                            new Product ("SP002", "Trà Xanh Hoa Nhài", 0, 12000, "ms-appx:///Assets/tea2.jpg"),
                            new Product ("SP003", "Trà Sữa Lài", 0, 20000, "ms-appx:///Assets/tea3.jpg")
                        };
            }

        }

        public IRepository<Product> Categories { get; set; } = new MockCategoryRepository();
    }
}
