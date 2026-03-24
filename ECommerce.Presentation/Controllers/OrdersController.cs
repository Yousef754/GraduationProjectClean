using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// إنشاء أوردر جديد
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder(OrderDTO orderDTO)
        {
            var email = GetEmailFromToken();
            var result = await _orderService.CreateOrderAsync(orderDTO, email);
            return HandleResult(result);
        }

        /// <summary>
        /// جلب كل الأوردرات الخاصة بالمستخدم الحالي
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetOrders()
        {
            var email = GetEmailFromToken();
            var result = await _orderService.GetAllOrdersAsync(email);
            return HandleResult(result);
        }

        /// <summary>
        /// جلب أوردر محدد بالـ Id للمستخدم الحالي
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrder(Guid id)
        {
            var email = GetEmailFromToken();
            var result = await _orderService.GetOrderByIdAsync(id, email);
            return HandleResult(result);
        }

        /// <summary>
        /// جلب كل طرق التوصيل المتاحة (مفتوحة للجميع)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var result = await _orderService.GetAllDeliveryMethodsAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// دالة مساعدة لاستخراج الإيميل من التوكن
        /// </summary>
        private string GetEmailFromToken()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        }
    }
}
