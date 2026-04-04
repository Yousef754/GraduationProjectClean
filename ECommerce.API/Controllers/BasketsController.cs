using ECommerce.Domain.Contracts;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.BasketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        // ==========================
        // 1. Get Basket
        // GET: api/basket
        // ==========================
        [HttpGet("GetBasket")]
        public async Task<ActionResult<BasketDTO>> GetBasket()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var basket = await _basketService.GetBasketAsync(userId);

            return Ok(basket);
        }

        // ==========================
        // 2. Add or Update Item
        // POST: api/basket/items
        // ==========================
        [HttpPost("OpenBasketWithAddOrUpdateItem")]
        public async Task<ActionResult<BasketDTO>> UpdateItem([FromBody] BasketItemDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is NULL from token");

            var basket = await _basketService.UpdateItemQuantityAsync(
                userId,
                dto.ProductId,
                dto.Quantity
            );

            return Ok(basket);
        }

        // ==========================
        // 3. Remove Item
        // DELETE: api/basket/items/{productId}
        // ==========================
        [HttpDelete("Deleteitems/{productId}")]
        public async Task<ActionResult<BasketDTO>> RemoveItem(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var basket = await _basketService.UpdateItemQuantityAsync(
                userId,
                productId,
                0
            );

            return Ok(basket);
        }

        // ==========================
        // 4. Update Delivery Method
        // POST: api/basket/delivery
        // ==========================
        [HttpPost("UpdateDelivery")]
        public async Task<ActionResult<BasketDTO>> UpdateDeliveryMethod([FromBody] int deliveryMethodId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var basket = await _basketService.UpdateDeliveryMethodAsync(
                userId,
                deliveryMethodId
            );

            return Ok(basket);
        }

        // ==========================
        // 5. Delete Basket
        // DELETE: api/basket
        // ==========================
        [HttpDelete("DeleteBasket")]
        public async Task<IActionResult> DeleteBasket()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var success = await _basketService.DeleteBasketAsync(userId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
