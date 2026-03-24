using ECommerce.Domain.Contracts;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.BasketDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        // ==========================
        // إنشاء سلة مع منتجات
        // POST: api/basket
        // ==========================
        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateBasket([FromBody] BasketDTO basket)
        {
            var result = await _basketService.CreateOrUpdateBasketAsync(basket);
            return Ok(result);
        }

        // ==========================
        // جلب السلة
        // GET: api/basket/{id}
        // ==========================
        [HttpGet("{id}")]
        public async Task<ActionResult<BasketDTO>> GetBasket(string id)
        {
            try
            {
                var basket = await _basketService.GetBasketAsync(id);
                return Ok(basket);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ==========================
        // حذف السلة
        // DELETE: api/basket/{id}
        // ==========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasket(string id)
        {
            var success = await _basketService.DeleteBasketAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // ==========================
        // إضافة منتج للسلة
        // POST: api/basket/{id}/add-product
        // ==========================
        [HttpPost("{id}/addproduct")]
        public async Task<ActionResult<BasketDTO>> AddProductToBasket(
            string id,
            [FromQuery] int productId,
            [FromQuery] int quantity)
        {
            var basket = await _basketService.AddProductToBasketAsync(id, productId, quantity);
            return Ok(basket);
        }

        [HttpPost("{id}/updatedelivery")]
        public async Task<ActionResult<BasketDTO>> UpdateDeliveryMethod(
                string id,
                [FromQuery] int deliveryMethodId,
                 [FromServices] IUnitOfWork unitOfWork) // مهم عشان نجيب DeliveryMethod من DB
        {
            try
            {
                var basket = await _basketService.UpdateDeliveryMethodAsync(id, deliveryMethodId, unitOfWork);
                return Ok(basket);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
