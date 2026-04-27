using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.OrderDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ApiBaseController
    {
        private readonly IPaymobService _paymobService;

        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService, IPaymobService paymobService)
        {
            _orderService = orderService;
            _paymobService = paymobService;

        }

        /// <summary>
        /// إنشاء أوردر جديد
        /// </summary>
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderToReturnDTO>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            //var userid = GetEmailFromToken();

            //if (string.IsNullOrEmpty(userid))
            //    return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _orderService.CreateOrderAsync(dto, userId);

            return HandleResult(result);
        }

        /// <summary>
        /// جلب كل الأوردرات الخاصة بالمستخدم الحالي
        /// </summary>
        [HttpGet("GetAllOrdersWithSameUser")]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetOrders()
        {
            var email = GetEmailFromToken();

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

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

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var result = await _orderService.GetOrderByIdAsync(id, email);

            return HandleResult(result);
        }

        /// <summary>
        /// جلب كل طرق التوصيل (مفتوحة)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDTO>>> GetDeliveryMethods()
        {
            var result = await _orderService.GetAllDeliveryMethodsAsync();

            return HandleResult(result);
        }

        /// <summary>
        /// استخراج الإيميل من التوكن
        /// </summary>
        private string GetEmailFromToken()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        }

        [HttpPost("online/{id:guid}")]
        public async Task<ActionResult> PayOrder([FromRoute] Guid id)
        {
            // حط
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var orderResult = await _orderService.GetOrderByIdAsync(id, userId);
            if (!orderResult.IsSuccess)
                return BadRequest(orderResult.Errors);

            var order = orderResult.Value;

            // لو Cash مش محتاج دفع
            if (order.PaymentMethod == PaymentMethod.Cash)
                return BadRequest("Cash orders do not require payment");

            // عمل Order object مؤقت عشان نبعته لـ Paymob
            var orderForPayment = new Order
            {
                Phone = order.Phone,
                Address = order.Address,
                SubTotal = order.Subtotal,
                DeliveryMethod = new DeliveryMethod { Price = order.DeliveryMethod.Price }
            };

            var paymentUrl = await _paymobService.GetPaymentKeyAsync(orderForPayment);
            return Ok(new { PaymentUrl = paymentUrl });
        }

    }
}
