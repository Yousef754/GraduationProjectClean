using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly PaymobSettings _settings;
        private readonly HttpClient _httpClient;

        public PaymobService(IOptions<PaymobSettings> settings, HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<string> GetPaymentKeyAsync(Order order)
        {
            // Step 1: Authentication
            var authResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = _settings.ApiKey });

            var authContent = await authResponse.Content.ReadFromJsonAsync<JsonElement>();
            var authToken = authContent.GetProperty("token").GetString();

            // Step 2: Create Order
            var orderResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = false,
                    amount_cents = (int)(order.GetTotal() * 100),
                    currency = "EGP",
                    items = new List<object>()
                });

            var orderContent = await orderResponse.Content.ReadFromJsonAsync<JsonElement>();
            var paymobOrderId = orderContent.GetProperty("id").GetInt64();

            // Step 3: Payment Key
            var billingData = new
            {
                first_name = "Customer",
                last_name = "Name",
                email = "customer@email.com",
                phone_number = order.Phone,
                apartment = "NA",
                floor = "NA",
                street = order.Address,
                building = "NA",
                shipping_method = "NA",
                postal_code = "NA",
                city = "Cairo",
                country = "EG",
                state = "Cairo"
            };

            var paymentKeyResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = (int)(order.GetTotal() * 100),
                    expiration = 3600,
                    order_id = paymobOrderId,
                    billing_data = billingData,
                    currency = "EGP",
                    integration_id = int.Parse(_settings.IntegrationId)
                });

            var paymentKeyContent = await paymentKeyResponse.Content.ReadFromJsonAsync<JsonElement>();
            var paymentKey = paymentKeyContent.GetProperty("token").GetString();

            // Return IFrame URL
            return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentKey}";
        }
    }
}
