using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class ChatbotService
    {
        private readonly HttpClient _httpClient;

        public ChatbotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendMessageAsync(string message, List<List<string>> history)
        {
            var payload = new { message, history };
            var response = await _httpClient.PostAsJsonAsync(
                "https://mahmoud26378-laptop-chatbot.hf.space/chat", payload);

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (result.TryGetProperty("reply", out var reply))
                return reply.GetString()!;

            return "عذراً، حدث خطأ في الاتصال بالـ chatbot";
        }
    }
}
