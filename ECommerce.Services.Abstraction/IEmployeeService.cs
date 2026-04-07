using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.EmployeeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface IEmployeeService
    {
        Task<Result<IEnumerable<EmployeeToReturnDto>>> GetAllEmployeesAsync();
        Task<Result<EmployeeToReturnDto>> GetEmployeeByIdAsync(int id);
        Task<Result<EmployeeToReturnDto>> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task<Result<EmployeeToReturnDto>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
        Task<Result<bool>> DeleteEmployeeAsync(int id);
    }
}
