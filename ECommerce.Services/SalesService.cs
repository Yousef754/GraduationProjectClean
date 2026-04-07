using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Entities.Sales;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications.SaleSpecific;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.SalesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IGenericRepository<Sale, int> Repo
            => _unitOfWork.GetRepository<Sale, int>();

        public async Task<Result<IEnumerable<SaleToReturnDto>>> GetAllSalesAsync()
        {
            var spec = new SaleWithRelationsSpecification();
            var sales = await Repo.GetAllAsync(spec);
            return Result<IEnumerable<SaleToReturnDto>>.Ok(_mapper.Map<IEnumerable<SaleToReturnDto>>(sales));

        }

        public async Task<Result<SaleToReturnDto>> GetSaleByIdAsync(int id)
        {
            var spec = new SaleWithRelationsSpecification(id);
            var sale = await Repo.GetByIdAsync(spec);
            if (sale is null)
                return Error.NotFound("Sale.NotFound", "Sale not found");

            return Result<SaleToReturnDto>.Ok(_mapper.Map<SaleToReturnDto>(sale));

        }

        public async Task<Result<SaleToReturnDto>> CreateSaleAsync(CreateSaleDto dto)
        {
            // جيب المنتج
            var product = await _unitOfWork
                .GetRepository<Product, int>()
                .GetByIdAsync(dto.ProductId);
            if (product is null)
                return Error.NotFound("Product.NotFound", "Product not found");

            // جيب الموظف
            var employee = await _unitOfWork
                .GetRepository<Employee, int>()
                .GetByIdAsync(dto.EmployeeId);
            if (employee is null)
                return Error.NotFound("Employee.NotFound", "Employee not found");

            // لو Completed ننقص الكمية
            if (dto.Status == SaleStatus.Completed)
            {
                if (product.Quantity < dto.Quantity)
                    return Error.Validation("Product.OutOfStock", $"{product.Name} is out of stock");

                product.Quantity -= dto.Quantity;
                _unitOfWork.GetRepository<Product, int>().Update(product);
            }

            var sale = new Sale
            {
                CustomerName = dto.CustomerName,
                EmployeeId = dto.EmployeeId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = product.Price,
                TotalAmount = product.Price * dto.Quantity,
                Status = dto.Status
            };

            await Repo.AddAsync(sale);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Sale.Failure", "Failed to create sale");

            // لود الـ relations للـ mapping
            sale.Employee = employee;
            sale.Product = product;

            return Result<SaleToReturnDto>.Ok(_mapper.Map<SaleToReturnDto>(sale));
        }

        public async Task<Result<SaleToReturnDto>> UpdateSaleAsync(int id, UpdateSaleDto dto)
        {
            var sale = await Repo.GetByIdAsync(id);
            if (sale is null)
                return Error.NotFound("Sale.NotFound", "Sale not found");

            // مينفعش تعدل لو Completed أو Cancelled
            if (sale.Status == SaleStatus.Completed || sale.Status == SaleStatus.Cancelled)
                return Error.Validation("Sale.CannotUpdate", "Cannot update a completed or cancelled sale");

            // لو بيغير المنتج أو الكمية
            if (dto.ProductId is not null || dto.Quantity is not null)
            {
                var productId = dto.ProductId ?? sale.ProductId;
                var product = await _unitOfWork
                    .GetRepository<Product, int>()
                    .GetByIdAsync(productId);
                if (product is null)
                    return Error.NotFound("Product.NotFound", "Product not found");

                sale.ProductId = productId;
                sale.Quantity = dto.Quantity ?? sale.Quantity;
                sale.Price = product.Price;
                sale.TotalAmount = product.Price * sale.Quantity;
            }

            // لو بيغير الستاتوس لـ Completed ننقص الكمية
            if (dto.Status == SaleStatus.Completed)
            {
                var product = await _unitOfWork
                    .GetRepository<Product, int>()
                    .GetByIdAsync(sale.ProductId);
                if (product is null)
                    return Error.NotFound("Product.NotFound", "Product not found");

                if (product.Quantity < sale.Quantity)
                    return Error.Validation("Product.OutOfStock", $"{product.Name} is out of stock");

                product.Quantity -= sale.Quantity;
                _unitOfWork.GetRepository<Product, int>().Update(product);
            }

            if (dto.CustomerName is not null) sale.CustomerName = dto.CustomerName;
            if (dto.EmployeeId is not null) sale.EmployeeId = dto.EmployeeId.Value;
            if (dto.Status is not null) sale.Status = dto.Status.Value;

            Repo.Update(sale);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Sale.Failure", "Failed to update sale");

            sale.Employee = await _unitOfWork.GetRepository<Employee, int>().GetByIdAsync(sale.EmployeeId);
            sale.Product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(sale.ProductId);

            return Result<SaleToReturnDto>.Ok(_mapper.Map<SaleToReturnDto>(sale));
        }

        public async Task<Result<bool>> DeleteSaleAsync(int id)
        {
            var sale = await Repo.GetByIdAsync(id);
            if (sale is null)
                return Error.NotFound("Sale.NotFound", "Sale not found");

            Repo.Delete(sale);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Sale.Failure", "Failed to delete sale");

            return Result<bool>.Ok(true);
        }
    }
}
