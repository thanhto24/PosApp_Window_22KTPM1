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
                    new Product ("Hồng Trà Đài Loan", "12,000đ", "ms-appx:///Assets/tea1.jpg"),
                    new Product ("Trà Xanh Hoa Nhài", "12,000đ", "ms-appx:///Assets/tea2.jpg"),
                    new Product ("Trà Sữa Lài", "20,000đ", "ms-appx:///Assets/tea3.jpg")
                };
            }
        }

        public IRepository<Product> Categories { get; set; } = new MockCategoryRepository();


        public class MockOrderRepository : IRepository<Order> {
            public List<Order> GetAll()
            {
                return new List<Order>()
                {
                    new Order(1, "INV001", "Nguyễn Văn A", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Lê Hoàng", "500000", "50000", "450000", "300000", "Tiền mặt",
                              "Đã giao", "Đã thanh toán", "Giao hàng thành công"),

                    new Order(2, "INV002", "Trần Thị B", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Vũ Minh", "700000", "100000", "600000", "400000", "Chuyển khoản",
                              "Chưa giao", "Chờ xác nhận", "Chưa xác nhận thanh toán"),

                    new Order(3, "INV003", "Phạm Văn C", DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Ngô Thảo", "900000", "200000", "700000", "500000", "Thẻ tín dụng",
                              "Đã giao", "Đã thanh toán", "Khách yêu cầu xuất hóa đơn"),

                    new Order(4, "INV004", "Lý Hữu D", DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Đinh Phương", "1200000", "150000", "1050000", "700000", "Tiền mặt",
                              "Đang xử lý", "Chờ thanh toán", "Chờ xác nhận từ quản lý"),

                    new Order(5, "INV005", "Đoàn Thanh E", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                              "Trương Linh", "2000000", "500000", "1500000", "1200000", "Ví điện tử",
                              "Đã hủy", "Hoàn tiền", "Khách hàng hủy đơn do thay đổi nhu cầu")
                };
            }
        }
        public IRepository<Order> Orders { get; set; } = new MockOrderRepository();

        public class MockVoucherRepository : IRepository<Voucher>
        {
            public List<Voucher> GetAll()
            {
                return new List<Voucher>() {
                    new Voucher("1",DateTime.Now, DateTime.Now, 100, "aaa", 1,2,3),
                    new Voucher("2",DateTime.Now, DateTime.Now, 100, "bbb", 1,2,3),
                };
            }
        }

        public IRepository<Voucher> Vouchers { get; set; } = new MockVoucherRepository();
    }
}
