using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Service
{
    public class Payos
    {
        public class PaymentResponse
        {
            public string qrCode { get; set; }
            public long orderCode { get; set; }
            public string checkoutUrl { get; set; }
        }
        public class PaymentService
        {
            private readonly HttpClient _httpClient = new HttpClient();
            private const string BaseUrl = "http://localhost:5678/payos";

            public async Task<PaymentResponse?> CreatePayment(int amount)
            {
                var requestBody = new { amount = amount };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/create-payment", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaymentResponse>(json);
                }

                return null;
            }

            public async Task<bool> CheckPaymentStatus(long orderCode)
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/order-status/{orderCode}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<string>(json);
                    return result != null && result == "PAID";
                }

                return false;
            }
        }

    }
}
