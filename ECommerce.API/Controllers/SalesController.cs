using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.SalesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ApiBaseController
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet("GetAllSales")]
        public async Task<ActionResult<IEnumerable<SaleToReturnDto>>> GetAll()
        {
            var result = await _saleService.GetAllSalesAsync();
            return HandleResult(result);
        }

        [HttpGet("GetSaleById/{id}")]
        public async Task<ActionResult<SaleToReturnDto>> GetById(int id)
        {
            var result = await _saleService.GetSaleByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost("CreateSale")]
        public async Task<ActionResult<SaleToReturnDto>> Create([FromBody] CreateSaleDto dto)
        {
            var result = await _saleService.CreateSaleAsync(dto);
            return HandleResult(result);
        }

        [HttpPatch("UpdateSale/{id}")]
        public async Task<ActionResult<SaleToReturnDto>> Update(int id, [FromBody] UpdateSaleDto dto)
        {
            var result = await _saleService.UpdateSaleAsync(id, dto);
            return HandleResult(result);
        }

        [HttpDelete("DeleteSale/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _saleService.DeleteSaleAsync(id);
            return HandleResult(result);
        }
    }
}
