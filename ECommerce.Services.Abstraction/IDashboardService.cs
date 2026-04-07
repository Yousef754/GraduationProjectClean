using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface IDashboardService
    {
        Task<Result<DashboardDto>> GetDashboardAsync();
    }
}
