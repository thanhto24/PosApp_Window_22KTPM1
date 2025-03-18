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
        //    private static string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");
        //    public class SQLCategoryRepository : IRepository<Product>
        //    {
        //        public List<Product> GetAll()
        //        {
        //            return new List<Product>()
        //            {

        //            };
        //        }

        //        public 
        //    }

        //    public IRepository<Product> Categories { get; set; } = new SQLCategoryRepository();


        //    public class SQLOrderRepository : IRepository<Order>
        //    {
        //        public List<Order> GetAll()
        //        {
        //            return new List<Order>()
        //            {

        //            };
        //        }
        //    }
        //    public IRepository<Order> Orders { get; set; } = new SQLOrderRepository();

        //    public class SQLVoucherRepository : IRepository<Voucher>
        //    {
        //        public List<Voucher> GetAll()
        //        {
        //            List<Voucher> vouchers = new List<Voucher>();

        //            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
        //            {
        //                connection.Open();
        //                var command = connection.CreateCommand();
        //                command.CommandText = "SELECT * FROM Vouchers";

        //                using (var reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        vouchers.Add(new Voucher(
        //                            reader.GetString(1),
        //                            reader.GetDateTime(2),
        //                            reader.GetDateTime(3),
        //                            reader.GetInt32(4),
        //                            reader.IsDBNull(5) ? "" : reader.GetString(5),
        //                            reader.GetDecimal(6),
        //                            reader.GetDecimal(7))
        //                        );
        //                    }
        //                }
        //            }

        //            return vouchers;
        //        }

        //        public async System.Threading.Tasks.Task CreateVoucher(string code, DateTimeOffset? startDate, DateTimeOffset? endDate, int quantity, decimal minOrder, decimal discountValue, string note)
        //        {
        //            try
        //            {
        //                using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
        //                {
        //                    await connection.OpenAsync();

        //                    using (var command = connection.CreateCommand())
        //                    {
        //                        command.CommandText = @"
        //                        INSERT INTO Vouchers (Code, StartDate, EndDate, Quantity, Note, MinOrderValue, DiscountValue)
        //                        VALUES (@code, @startDate, @endDate, @quantity, @note, @minOrderValue, @discountValue);
        //                    ";

        //                        command.Parameters.AddWithValue("@code", code);
        //                        command.Parameters.AddWithValue("@startDate", startDate);
        //                        command.Parameters.AddWithValue("@endDate", endDate);
        //                        command.Parameters.AddWithValue("@quantity", quantity);
        //                        command.Parameters.AddWithValue("@note", note);
        //                        command.Parameters.AddWithValue("@minOrderValue", minOrder);
        //                        command.Parameters.AddWithValue("@discountValue", discountValue);

        //                        await command.ExecuteNonQueryAsync();
        //                    }
        //                }

        //                //await ShowMessage("Mã giảm giá đã được tạo thành công!");
        //            }
        //            catch (Exception ex)
        //            {
        //                //await ShowMessage($"Lỗi khi tạo mã giảm giá: {ex.Message}");
        //            }
        //        }

        //    }

        //    public IRepository<Voucher> Vouchers { get; set; } = new SQLVoucherRepository();
        /////////////////////
        public IRepository<Product> Categories { get; set; } = new SQLRepository<Product>();
        public IRepository<Order> Orders { get; set; } = new SQLRepository<Order>();
        public IRepository<Voucher> Vouchers { get; set; } = new SQLRepository<Voucher>();

    }
}
