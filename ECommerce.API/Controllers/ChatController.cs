using ECommerce.Services;
using Microsoft.AspNetCore.Mvc;
using static ECommerce.Shared.DTOs.Chatbot.ChatbotDto;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;

        public ChatController(ChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
        {
            var reply = await _chatbotService.SendMessageAsync(request.Message, request.History);
            return Ok(new ChatResponse(reply));
        }
    }
}
