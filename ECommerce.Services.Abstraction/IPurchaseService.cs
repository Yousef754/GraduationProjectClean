using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.PurchaseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface IPurchaseService
    {
        Task<Result<IEnumerable<PurchaseToReturnDto>>> GetAllPurchasesAsync();
        Task<Result<PurchaseToReturnDto>> GetPurchaseByIdAsync(int id);
        Task<Result<PurchaseToReturnDto>> CreatePurchaseAsync(CreatePurchaseDto dto);
        Task<Result<PurchaseToReturnDto>> UpdatePurchaseAsync(int id, UpdatePurchaseDto dto);
        Task<Result<bool>> DeletePurchaseAsync(int id);
    }
}
