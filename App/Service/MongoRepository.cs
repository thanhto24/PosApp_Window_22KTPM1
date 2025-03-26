using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Service
{
    public class MongoRepository<T> : IRepository<T> where T : class, new()
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "http://localhost:5678/"; // Thay bằng API backend của bạn
        private readonly string _modelName;

        public MongoRepository()
        {
            _modelName = typeof(T).Name.ToLower(); // Chuyển model thành chữ thường
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(BaseUrl);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            //_httpClient.BaseAddress = new Uri(BaseUrl);
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public List<T> GetAll()
        {
            return LoadData(); // Gọi API đồng bộ
        }

        private List<T> LoadData()
        {
            try
            {
                string url = $"{_modelName}";
                Console.WriteLine($"[LoadData] Calling API: {url}");

                var response = _httpClient.GetAsync(url).Result; // Gọi API đồng bộ
                Console.WriteLine($"[LoadData] Response: {(int)response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[LoadData] API Error: {response.ReasonPhrase}");
                    return new List<T>();
                }

                var jsonString = response.Content.ReadAsStringAsync().Result; // Đọc JSON dưới dạng string
                var jsonData = JsonSerializer.Deserialize<List<T>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (jsonData == null) return new List<T>();

                Console.WriteLine($"[LoadData] Fetched {jsonData.Count} records.");
                foreach (var item in jsonData)
                {
                    Console.WriteLine(item); // In từng object
                }

                return jsonData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadData] Error: {ex.Message}");
                return new List<T>();
            }
        }
        public void Insert(T entity)
        {
            try
            {
                string url = $"{_modelName}"; // API mới dùng POST /:modelName
                Console.WriteLine($"[Insert] Calling API: {url}");

                var response = _httpClient.PostAsJsonAsync(url, entity).Result;
                Console.WriteLine($"[Insert] Response: {(int)response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Insert] API Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Insert] Error: {ex.Message}");
            }
        }

        public void RemoveByQuery(string whereClause, Dictionary<string, object> parameters)
        {
            try
            {
                string url = $"rmByQuery/{_modelName}"; // API mới dùng DELETE /rmByQuery/:modelName
                Console.WriteLine($"[RemoveByQuery] Calling API: {url}");

                var request = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = JsonContent.Create(new { whereClause, parameters })
                };

                var response = _httpClient.SendAsync(request).Result;
                Console.WriteLine($"[RemoveByQuery] Response: {(int)response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[RemoveByQuery] API Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoveByQuery] Error: {ex.Message}");
            }
        }

        public void UpdateByQuery(Dictionary<string, object> setValues, string whereClause, Dictionary<string, object> whereParams)
        {
            try
            {
                string url = $"updateByQuery/{_modelName}"; // API mới dùng PUT /updateByQuery/:modelName
                Console.WriteLine($"[UpdateByQuery] Calling API: {url}");

                var payload = new { setValues, whereClause, whereParams };
                var response = _httpClient.PutAsJsonAsync(url, payload).Result;
                Console.WriteLine($"[UpdateByQuery] Response: {(int)response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[UpdateByQuery] API Error: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateByQuery] Error: {ex.Message}");
            }
        }

        public List<T> GetFiltered(string searchText = "", string productType = "Tất cả", string productGroup = "Tất cả", string status = "Tất cả", string sortOrder = "Tên: A => Z")
        {
            try
            {
                Debug.WriteLine("MongoRepository", $"Filters: {searchText}, {productType}, {productGroup}, {status}, {sortOrder}");
                string url = $"filtered/{_modelName}";
                Console.WriteLine($"[GetFiltered] Calling API: {url}");

                var payload = new
                {
                    searchText,
                    productType,
                    productGroup,
                    status,
                    sortOrder
                };

                var response = _httpClient.PostAsJsonAsync(url, payload).Result;
                Console.WriteLine($"[GetFiltered] Response: {(int)response.StatusCode} - {response.ReasonPhrase}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[GetFiltered] API Error: {response.ReasonPhrase}");
                    return new List<T>();
                }

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var jsonData = JsonSerializer.Deserialize<List<T>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return jsonData ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetFiltered] Error: {ex.Message}");
                return new List<T>();
            }
        }
    }
}
