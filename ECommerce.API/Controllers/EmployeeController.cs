using ECommerce.Services.Abstraction;
using ECommerce.Shared.DTOs.EmployeeDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ApiBaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("GetAllEmployee")]
        public async Task<ActionResult<IEnumerable<EmployeeToReturnDto>>> GetAll()
        {
            var result = await _employeeService.GetAllEmployeesAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeToReturnDto>> GetById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost("CreateEmployee")]
        public async Task<ActionResult<EmployeeToReturnDto>> Create([FromBody] CreateEmployeeDto dto)
        {
            var result = await _employeeService.CreateEmployeeAsync(dto);
            return HandleResult(result);
        }

        [HttpPatch("UpdateEmployee/{id}")]
        public async Task<ActionResult<EmployeeToReturnDto>> Update(int id, [FromBody] UpdateEmployeeDto dto)
        {
            var result = await _employeeService.UpdateEmployeeAsync(id, dto);
            return HandleResult(result);
        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            return HandleResult(result);
        }
    }
}
