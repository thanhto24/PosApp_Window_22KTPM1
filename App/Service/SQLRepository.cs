using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace App.Service
{
    public class SQLRepository<T> : IRepository<T> where T : class, new()
    {
        private string DatabasePath = Path.Combine(AppContext.BaseDirectory, "database.db");

        public List<T> GetAll()
        {
            List<T> list = new List<T>();

            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {typeof(T).Name}"; // Lấy tên bảng từ class T

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T entity = new T();
                        foreach (var prop in typeof(T).GetProperties()) // Lấy danh sách property của T
                        {
                            int colIndex;
                            try
                            {
                                colIndex = reader.GetOrdinal(prop.Name); // Lấy cột theo tên property
                            }
                            catch (IndexOutOfRangeException)
                            {
                                continue; // Bỏ qua nếu không tìm thấy cột
                            }

                            if (!reader.IsDBNull(colIndex))
                            {
                                if (prop.PropertyType == typeof(DateTimeOffset) || prop.PropertyType == typeof(DateTimeOffset?))
                                {
                                    string dateStr = reader.GetString(colIndex);
                                    if (DateTimeOffset.TryParse(dateStr, out DateTimeOffset dateValue))
                                    {
                                        prop.SetValue(entity, dateValue);
                                    }
                                }
                                else
                                {
                                    prop.SetValue(entity, Convert.ChangeType(reader.GetValue(colIndex), prop.PropertyType));
                                }
                            }
                        }

                        list.Add(entity);
                    }
                }
            }

            return list;
        }

        public List<T> GetFiltered(string searchText = "", string productType = "Tất cả", string productGroup = "Tất cả", string status = "Tất cả", string sortOrder = "Tên: A => Z")
        {
            throw new NotImplementedException();
        }

        public void Insert(T entity)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Lấy danh sách cột từ class T
                var properties = typeof(T).GetProperties()
                                          .Where(p => p.Name != "Id") // Loại bỏ cột ID (nếu có)
                                          .ToList();

                // Tạo danh sách tên cột và giá trị tương ứng
                string columns = string.Join(", ", properties.Select(p => p.Name));
                string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                // Tạo câu lệnh SQL INSERT
                command.CommandText = $"INSERT INTO {typeof(T).Name} ({columns}) VALUES ({values})";

                // Gán giá trị cho từng tham số
                foreach (var prop in properties)
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(entity) ?? DBNull.Value);
                }

                // Thực thi lệnh
                command.ExecuteNonQuery();
            }
        }

        public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Tạo câu lệnh DELETE với điều kiện
                command.CommandText = $"DELETE FROM {typeof(T).Name} WHERE {whereClause}";

                // Gán giá trị cho từng tham số
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }

                // Thực thi lệnh
                command.ExecuteNonQuery();
            }
        }

        public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Tạo danh sách các cặp "column = @param"
                string setClause = string.Join(", ", setValues.Keys.Select(key => $"{key} = @{key}"));

                // Câu lệnh UPDATE SQL
                command.CommandText = $"UPDATE {typeof(T).Name} SET {setClause} WHERE {whereClause}";

                // Thêm tham số cho giá trị cập nhật
                foreach (var param in setValues)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }

                // Thêm tham số cho điều kiện WHERE
                foreach (var param in whereParams)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }

                // Thực thi lệnh
                command.ExecuteNonQuery();
            }
        }

        public List<T> GetByQuery(Dictionary<string, object> filter, Dictionary<string, object>? or = null, Dictionary<string, int>? sort = null)
        {
            throw new NotImplementedException();
        }
    }
}
