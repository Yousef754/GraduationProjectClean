using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.Chatbot
{
    public class ChatbotDto
    {
        public record ChatRequest(string Message, List<List<string>> History);
        public record ChatResponse(string Reply);
    }
}
