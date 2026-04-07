using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.PurchaseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ApiBaseController
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet("GetAllPurchase")]
        public async Task<ActionResult<IEnumerable<PurchaseToReturnDto>>> GetAll()
        {
            var result = await _purchaseService.GetAllPurchasesAsync();
            return HandleResult(result);
        }

        [HttpGet("GetPurchaseById/{id}")]
        public async Task<ActionResult<PurchaseToReturnDto>> GetById(int id)
        {
            var result = await _purchaseService.GetPurchaseByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost("CreatePurchase")]
        public async Task<ActionResult<PurchaseToReturnDto>> Create([FromBody] CreatePurchaseDto dto)
        {
            var result = await _purchaseService.CreatePurchaseAsync(dto);
            return HandleResult(result);
        }

        [HttpPatch("UpdatePurchase/{id}")]
        public async Task<ActionResult<PurchaseToReturnDto>> Update(int id, [FromBody] UpdatePurchaseDto dto)
        {
            var result = await _purchaseService.UpdatePurchaseAsync(id, dto);
            return HandleResult(result);
        }

        [HttpDelete("DeletePurchase/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);
            return HandleResult(result);
        }
    }
}
