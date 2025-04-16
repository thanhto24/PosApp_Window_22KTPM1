using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App.Service
{
    public class GHN
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string baseUrl = "http://localhost:5678/ghn";
        public class AddressService
        {

            public async Task<List<Province>> GetProvincesAsync()
            {
                var response = await _httpClient.GetStringAsync($"{baseUrl}/provinces");
                var provinces = JsonConvert.DeserializeObject<List<Province>>(response);
                return provinces;
            }

            public async Task<List<District>> GetDistrictsAsync(int provinceId)
            {
                var response = await _httpClient.GetStringAsync($"{baseUrl}/districts?province_id={provinceId}");
                var districts = JsonConvert.DeserializeObject<List<District>>(response);
                return districts;
            }

            public async Task<List<Ward>> GetWardsAsync(int districtId)
            {
                var response = await _httpClient.GetStringAsync($"{baseUrl}/wards?district_id={districtId}");
                var wards = JsonConvert.DeserializeObject<List<Ward>>(response);
                return wards;
            }
        }


        public class Province
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class District
        {
            public int id { get; set; }
            public int provinceId { get; set; }
            public string name { get; set; }
        }
        public class Ward
        {
            public int id { get; set; }
            public int districtId { get; set; }
            public string name { get; set; }
        }

        public class ShipService
        {
            public class CreateOrderRequest
            {
                public string to_name { get; set; }
                public string to_phone { get; set; }
                public string to_address { get; set; }
                public string to_ward_code { get; set; }
                public int to_district_id { get; set; }
                public int cod_amount { get; set; } = 0;
            }

            public async Task<OrderShipResponse> CreateOrderShip(CreateOrderRequest order)
            {
                var jsonContent = JsonConvert.SerializeObject(order);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{baseUrl}/create-order", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Lỗi tạo đơn hàng: {error}");

                    return new OrderShipResponse();
                }

                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OrderShipResponse>(result);
            }

            public static async Task<int> GetShipFeeAsync(string toWardCode, int toDistrictId)
            {
                var payload = new
                {
                    to_ward_code = toWardCode,
                    to_district_id = toDistrictId
                };

                var jsonContent = JsonConvert.SerializeObject(payload);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{baseUrl}/ship-fee", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Lỗi lấy phí ship: {error}");
                }

                var result = await response.Content.ReadAsStringAsync();

                // Parse JSON và lấy total_fee
                var json = JsonConvert.DeserializeObject<Dictionary<string, int>>(result);
                return json.TryGetValue("total_fee", out var fee) ? fee : -1;
            }




            public class OrderShipResponse()
            {
                public string order_code { get; set; }
                public int total_fee { get; set; }

            }


        }



    }
}
