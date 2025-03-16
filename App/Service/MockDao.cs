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
                    new Product ("Hồng Trà Đài Loan", "12,000đ", "ms-appx:///Assets/tea1.jpg", "Đồ uống", 0, "10.000đ", ""),
                    new Product ("Trà Xanh Hoa Nhài", "12,000đ", "ms-appx:///Assets/tea2.jpg", "Đồ uống", 0, "10.000đ", ""),
                    new Product ("Trà Sữa Lài", "20,000đ", "ms-appx:///Assets/tea3.jpg", "Đồ uống", 0, "10.000đ", "")
                };
            }
        }

        public IRepository<Product> Categories { get; set; } = new MockCategoryRepository();
    }
}
