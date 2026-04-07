using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.SalesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface ISaleService
    {
        Task<Result<IEnumerable<SaleToReturnDto>>> GetAllSalesAsync();
        Task<Result<SaleToReturnDto>> GetSaleByIdAsync(int id);
        Task<Result<SaleToReturnDto>> CreateSaleAsync(CreateSaleDto dto);
        Task<Result<SaleToReturnDto>> UpdateSaleAsync(int id, UpdateSaleDto dto);
        Task<Result<bool>> DeleteSaleAsync(int id);
    }
}
