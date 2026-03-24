using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        /// <summary>
        /// إنشاء أوردر جديد باستخدام OrderDTO وإيميل المستخدم
        /// </summary>
        /// <param name="orderDTO">بيانات الأوردر من الـ client</param>
        /// <param name="email">إيميل المستخدم</param>
        /// <returns>OrderToReturnDTO مغلف بـ Result</returns>
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(OrderDTO orderDTO, string email);

        /// <summary>
        /// جلب كل طرق التوصيل المتاحة
        /// </summary>
        /// <returns>قائمة DeliveryMethodDTO مغلفة بـ Result</returns>
        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetAllDeliveryMethodsAsync();

        /// <summary>
        /// جلب كل الأوردرات الخاصة بمستخدم معين
        /// </summary>
        /// <param name="email">إيميل المستخدم</param>
        /// <returns>قائمة OrderToReturnDTO مغلفة بـ Result</returns>
        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string email);

        /// <summary>
        /// جلب أوردر محدد لمستخدم معين
        /// </summary>
        /// <param name="Id">معرف الأوردر</param>
        /// <param name="email">إيميل المستخدم</param>
        /// <returns>OrderToReturnDTO مغلف بـ Result</returns>
        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid Id, string email);
    }
}
