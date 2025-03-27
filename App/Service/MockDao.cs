using App.Model;
using Microsoft.UI.Xaml.Controls;
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
            private List<Product> products = new List<Product>()
            {
                new Product("CF001", "Espresso", 0, 32000, 0, "ms-appx:///Assets/espresso.jpg", "Cà phê", 0.1f, 25000, "8938503270012"),
                new Product("CF002", "Cappuccino", 0, 35000, 0, "ms-appx:///Assets/cappuccino.jpg", "Cà phê", 0.1f, 27000, "8938503270029"),
                new Product("CF003", "Latte", 0, 40000, 0, "ms-appx:///Assets/latte.jpg", "Cà phê", 0.1f, 30000, "8938503270036"),
                new Product("ST001", "Sinh tố bơ", 0, 45000, 0, "ms-appx:///Assets/smoothie_avocado.jpg", "Sinh tố", 0.05f, 38000, "8938503270043"),
                new Product("ST002", "Sinh tố xoài", 0, 40000, 0, "ms-appx:///Assets/smoothie_mango.jpg", "Sinh tố", 0.05f, 35000, "8938503270050"),
                new Product("ST003", "Sinh tố dâu", 0, 42000, 0, "ms-appx:///Assets/smoothie_strawberry.jpg", "Sinh tố", 0.05f, 36000, "8938503270067"),
                new Product("NJ001", "Ép cam", 0, 35000, 0, "ms-appx:///Assets/juice_orange.jpg", "Nước ép", 0.08f, 28000, "8938503270074"),
                new Product("NJ002", "Ép dưa hấu", 0, 30000, 0, "ms-appx:///Assets/juice_watermelon.jpg", "Nước ép", 0.08f, 25000, "8938503270081"),
                new Product("NJ003", "Ép cà rốt", 0, 32000, 0, "ms-appx:///Assets/juice_carrot.jpg", "Nước ép", 0.08f, 26000, "8938503270098"),
                new Product("TS001", "Trà sữa", 0, 38000, 0, "ms-appx:///Assets/milktea_classic.jpg", "Trà sữa", 0.07f, 32000, "8938503270104"),
                new Product("TS002", "Hồng trà", 0, 40000, 0, "ms-appx:///Assets/milktea_blackpearl.jpg", "Trà sữa", 0.07f, 34000, "8938503270111"),
                new Product("TS003", "Lục trà", 0, 42000, 0, "ms-appx:///Assets/milktea_matcha.jpg", "Trà sữa", 0.07f, 35000, "8938503270128"),
                new Product("SN001", "Khoai tây chiên", 0, 30000, 0, "ms-appx:///Assets/snack_fries.jpg", "Đồ ăn vặt", 0.1f, 20000, "8938503270135"),
                new Product("SN002", "Gà rán", 0, 45000, 0, "ms-appx:///Assets/snack_friedchicken.jpg", "Đồ ăn vặt", 0.1f, 35000, "8938503270142"),
                new Product("SN003", "Sandwich", 0, 35000, 0, "ms-appx:///Assets/snack_sandwich.jpg", "Đồ ăn vặt", 0.1f, 28000, "8938503270159")
            };

            public List<Product> GetAll() => products;
            public List<Product> GetProductsByCategory(string category) => products.Where(p => p.TypeGroup == category).ToList();
            public void Insert(Product product) { }
            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }
        }

        public IRepository<Product> Products { get; set; } = new MockProductRepository();

        public class MockOrderRepository : IRepository<Order_>
        {
            private static List<Order_> orders = new List<Order_>()
            {
                    new Order_(1, "INV001", "Nguyễn Văn A", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                              new List<Product> { new Product("CF001", "Espresso", 1, 32000, 0, "", "Cà phê", 0.1f, 25000, ""), new Product("ST001", "Sinh tố bơ", 1, 45000, 0, "", "Sinh tố", 0.05f, 38000, "") }, 500000, 50000, 450000, 300000, "Tiền mặt",
                              "Đã giao", "Đã thanh toán", "Giao hàng thành công"),

                    new Order_(2, "INV002", "Trần Thị B", DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss"),
                              new List<Product> { new Product("TS002", "Hồng trà", 2, 40000, 0, "", "Trà sữa", 0.07f, 34000, "") }, 700000, 100000, 600000, 400000, "Chuyển khoản",
                              "Chưa giao", "Chờ xác nhận", "Chưa xác nhận thanh toán"),

                    new Order_(3, "INV003", "Phạm Văn C", DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss"),
                              new List<Product> { new Product("TS002", "Hồng trà", 2, 40000, 0, "", "Trà sữa", 0.07f, 34000, "") }, 900000, 200000, 700000, 500000, "Thẻ tín dụng",
                              "Đã giao", "Đã thanh toán", "Khách yêu cầu xuất hóa đơn"),

                    new Order_(4, "INV004", "Lý Hữu D", DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss"),
                              new List<Product> { new Product("TS002", "Hồng trà", 2, 40000, 0, "", "Trà sữa", 0.07f, 34000, "") }, 1200000, 150000, 1050000, 700000, "Tiền mặt",
                              "Đang xử lý", "Chờ thanh toán", "Chờ xác nhận từ quản lý"),

                    new Order_(5, "INV005", "Đoàn Thanh E", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss"),
                              new List<Product> { new Product("TS002", "Hồng trà", 2, 40000, 0, "", "Trà sữa", 0.07f, 34000, "") }, 2000000, 500000, 1500000, 1200000, "Ví điện tử",
                              "Đã hủy", "Hoàn tiền", "Khách hàng hủy đơn do thay đổi nhu cầu")
            };

            public List<Order_> GetAll() => orders;
            public void Insert(Order_ order) { orders.Add(order); }
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

            public List<Voucher> GetFiltered(string searchText = "", string productType = "Tất cả", string productGroup = "Tất cả", string status = "Tất cả", string sortOrder = "Tên: A => Z")
            {
                throw new NotImplementedException();
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
            public List<Category_> GetAll() => new List<Category_>()
            {
                new Category_("Cà phê"),
                new Category_("Sinh tố"),
                new Category_("Nước ép"),
                new Category_("Trà sữa"),
                new Category_("Đồ ăn vặt")
            };
            public void Insert(Category_ category) { }
            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }
        }
        public IRepository<Category_> Categories { get; set; } = new MockCategoryRepository();

        public class MockReportRepository : IRepository<ReportData>
        {
            private ReportData report = new ReportData(0, 0, 0);

            public List<ReportData> GetAll() => new List<ReportData> { report };

            public void Insert(ReportData newReport)
            {
                report = newReport;
            }

            public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters) { }
            public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams) { }
        }

        public IRepository<ReportData> Reports { get; set; } = new MockReportRepository();

    }
}