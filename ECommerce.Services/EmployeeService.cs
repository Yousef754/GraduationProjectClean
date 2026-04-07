using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.EmployeeDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IGenericRepository<Employee, int> Repo
            => _unitOfWork.GetRepository<Employee, int>();

        public async Task<Result<IEnumerable<EmployeeToReturnDto>>> GetAllEmployeesAsync()
        {
            var employees = await Repo.GetAllAsync();
            var result = _mapper.Map<IEnumerable<EmployeeToReturnDto>>(employees);
            return Result<IEnumerable<EmployeeToReturnDto>>.Ok(result);
        }

        public async Task<Result<EmployeeToReturnDto>> GetEmployeeByIdAsync(int id)
        {
            var employee = await Repo.GetByIdAsync(id);
            if (employee is null)
                return Error.NotFound("Employee.NotFound", "Employee not found");

            return Result<EmployeeToReturnDto>.Ok(_mapper.Map<EmployeeToReturnDto>(employee));
        }

        public async Task<Result<EmployeeToReturnDto>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            // تحقق من الإيميل مكررش
            var existing = await Repo.GetAllAsQueryable()
                .FirstOrDefaultAsync(e => e.Email == dto.Email);
            if (existing is not null)
                return Error.Validation("Employee.Conflict", "Email already exists");

            var employee = _mapper.Map<Employee>(dto);
            await Repo.AddAsync(employee);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Employee.Failure", "Failed to create employee");

            return Result<EmployeeToReturnDto>.Ok(_mapper.Map<EmployeeToReturnDto>(employee));
        }

        public async Task<Result<EmployeeToReturnDto>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await Repo.GetByIdAsync(id);
            if (employee is null)
                return Error.NotFound("Employee.NotFound", "Employee not found");

            if (dto.Name is not null) employee.Name = dto.Name;
            if (dto.Email is not null) employee.Email = dto.Email;
            if (dto.Phone is not null) employee.Phone = dto.Phone;
            if (dto.Position is not null) employee.Position = dto.Position;
            if (dto.Department is not null) employee.Department = dto.Department.Value;
            if (dto.Salary is not null) employee.Salary = dto.Salary.Value;
            if (dto.Status is not null) employee.Status = dto.Status.Value;
            if (dto.Permissions is not null) employee.Permissions = dto.Permissions;

            Repo.Update(employee);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Employee.Failure", "Failed to update employee");

            return Result<EmployeeToReturnDto>.Ok(_mapper.Map<EmployeeToReturnDto>(employee));
        }

        public async Task<Result<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await Repo.GetByIdAsync(id);
            if (employee is null)
                return Error.NotFound("Employee.NotFound", "Employee not found");

            Repo.Delete(employee);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Employee.Failure", "Failed to delete employee");

            return Result<bool>.Ok(true);
        }
    }
}
