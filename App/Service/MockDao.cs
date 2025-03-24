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
        public class MockProductRepository : IRepository<Product>
        {
            public List<Product> GetAll()
            {
                return new List<Product>() {
                    new Product ("Hồng Trà Đài Loan", 12000, "ms-appx:///Assets/tea1.jpg", "Đồ uống", 0, 100000, ""),
                    new Product ("Trà Xanh Hoa Nhài", 11000, "ms-appx:///Assets/tea2.jpg", "Đồ uống", 0, 20000, ""),
                    new Product ("Trà Sữa Lài", 10000, "ms-appx:///Assets/tea3.jpg", "Đồ uống", 0, 300, "")
                };
            }

            public void Insert(Product product)
            {
            }
            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }

        }

        public IRepository<Product> Products { get; set; } = new MockProductRepository();


        public class MockOrderRepository : IRepository<Order_>
        {
            public List<Order_> GetAll()
            {
                return new List<Order_>()
                {
                    new Order_(1, "INV001", "Nguyễn Văn A", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Lê Hoàng", 500000, 50000, 450000, 300000, "Tiền mặt",
                              "Đã giao", "Đã thanh toán", "Giao hàng thành công"),

                    new Order_(2, "INV002", "Trần Thị B", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Vũ Minh", 700000, 100000, 600000, 400000, "Chuyển khoản",
                              "Chưa giao", "Chờ xác nhận", "Chưa xác nhận thanh toán"),

                    new Order_(3, "INV003", "Phạm Văn C", DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Ngô Thảo", 900000, 200000, 700000, 500000, "Thẻ tín dụng",
                              "Đã giao", "Đã thanh toán", "Khách yêu cầu xuất hóa đơn"),

                    new Order_(4, "INV004", "Lý Hữu D", DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Đinh Phương", 1200000, 150000, 1050000, 700000, "Tiền mặt",
                              "Đang xử lý", "Chờ thanh toán", "Chờ xác nhận từ quản lý"),

                    new Order_(5, "INV005", "Đoàn Thanh E", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Trương Linh", 2000000, 500000, 1500000, 1200000, "Ví điện tử",
                              "Đã hủy", "Hoàn tiền", "Khách hàng hủy đơn do thay đổi nhu cầu")
                };
            }

            public void Insert(Order_ order)
            {
            }

            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }
        }
        public IRepository<Order_> Orders { get; set; } = new MockOrderRepository();

        public class MockVoucherRepository : IRepository<Voucher>
        {
            public List<Voucher> GetAll()
            {
                return new List<Voucher>() {
                    new Voucher("1",DateTime.Now, DateTime.Now, 100, 1, 1, "abc"),
                    new Voucher("2",DateTime.Now, DateTime.Now, 111, 2, 3, "def"),
                };
            }

            public void Insert(Voucher voucher)
            {
            }
            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }

        }

        public IRepository<Voucher> Vouchers { get; set; } = new MockVoucherRepository();

        public class MockCategoryRepository : IRepository<Category_>
        {
            public List<Category_> GetAll()
            {
                return new List<Category_>() {
             new Category_ ("Tra sua"),
             new Category_ ("Tra sua 2"),
             new Category_ ("Tra sua 3"),
         };
            }

            public void Insert(Category_ category)
            {
            }
            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }

        }

        public IRepository<Category_> Categories { get; set; } = new MockCategoryRepository();
    }
}
