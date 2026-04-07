using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Domain.Entities.ParchaseModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Specifications.PurchaseSpecification;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.PurchaseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IGenericRepository<Purchase, int> Repo
            => _unitOfWork.GetRepository<Purchase, int>();

        public async Task<Result<IEnumerable<PurchaseToReturnDto>>> GetAllPurchasesAsync()
        {
            var spec = new PurchaseWithRelationsSpecification();
            var purchases = await Repo.GetAllAsync(spec);
            return Result<IEnumerable<PurchaseToReturnDto>>.Ok(_mapper.Map<IEnumerable<PurchaseToReturnDto>>(purchases));
        }

        public async Task<Result<PurchaseToReturnDto>> GetPurchaseByIdAsync(int id)
        {
            var spec = new PurchaseWithRelationsSpecification(id);
            var purchase = await Repo.GetByIdAsync(spec);
            if (purchase is null)
                return Error.NotFound("Purchase.NotFound", "Purchase not found");

            return Result<PurchaseToReturnDto>.Ok(_mapper.Map<PurchaseToReturnDto>(purchase));
        }

        public async Task<Result<PurchaseToReturnDto>> CreatePurchaseAsync(CreatePurchaseDto dto)
        {
            var product = await _unitOfWork
                .GetRepository<Product, int>()
                .GetByIdAsync(dto.ProductId);
            if (product is null)
                return Error.NotFound("Product.NotFound", "Product not found");

            var employee = await _unitOfWork
                .GetRepository<Employee, int>()
                .GetByIdAsync(dto.EmployeeId);
            if (employee is null)
                return Error.NotFound("Employee.NotFound", "Employee not found");

            // لو Received نزود الكمية
            if (dto.Status == PurchaseStatus.ReceivedOrder)
            {
                product.Quantity += dto.Quantity;
                _unitOfWork.GetRepository<Product, int>().Update(product);
            }

            var purchase = new Purchase
            {
                SupplierName = dto.SupplierName,
                EmployeeId = dto.EmployeeId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = dto.Price,
                TotalAmount = dto.Price * dto.Quantity,
                Status = dto.Status
            };

            await Repo.AddAsync(purchase);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Purchase.Failure", "Failed to create purchase");

            purchase.Employee = employee;
            purchase.Product = product;

            return Result<PurchaseToReturnDto>.Ok(_mapper.Map<PurchaseToReturnDto>(purchase));
        }

        public async Task<Result<PurchaseToReturnDto>> UpdatePurchaseAsync(int id, UpdatePurchaseDto dto)
        {
            var purchase = await Repo.GetByIdAsync(id);
            if (purchase is null)
                return Error.NotFound("Purchase.NotFound", "Purchase not found");

            // مينفعش تعدل لو Received
            if (purchase.Status == PurchaseStatus.ReceivedOrder)
                return Error.Validation("Purchase.CannotUpdate", "Cannot update a received order");

            // لو بيغير المنتج أو الكمية
            if (dto.ProductId is not null || dto.Quantity is not null || dto.Price is not null)
            {
                var productId = dto.ProductId ?? purchase.ProductId;
                var product = await _unitOfWork
                    .GetRepository<Product, int>()
                    .GetByIdAsync(productId);
                if (product is null)
                    return Error.NotFound("Product.NotFound", "Product not found");

                purchase.ProductId = productId;
                purchase.Quantity = dto.Quantity ?? purchase.Quantity;
                purchase.Price = dto.Price ?? purchase.Price;
                purchase.TotalAmount = purchase.Price * purchase.Quantity;
            }

            // لو بيغير الستاتوس لـ Received نزود الكمية
            if (dto.Status == PurchaseStatus.ReceivedOrder)
            {
                var product = await _unitOfWork
                    .GetRepository<Product, int>()
                    .GetByIdAsync(purchase.ProductId);
                if (product is null)
                    return Error.NotFound("Product.NotFound", "Product not found");

                product.Quantity += purchase.Quantity;
                _unitOfWork.GetRepository<Product, int>().Update(product);
            }

            if (dto.SupplierName is not null) purchase.SupplierName = dto.SupplierName;
            if (dto.EmployeeId is not null) purchase.EmployeeId = dto.EmployeeId.Value;
            if (dto.Status is not null) purchase.Status = dto.Status.Value;

            Repo.Update(purchase);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Purchase.Failure", "Failed to update purchase");

            // لود الـ relations
            purchase.Employee = await _unitOfWork.GetRepository<Employee, int>().GetByIdAsync(purchase.EmployeeId);
            purchase.Product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(purchase.ProductId);

            return Result<PurchaseToReturnDto>.Ok(_mapper.Map<PurchaseToReturnDto>(purchase));
        }

        public async Task<Result<bool>> DeletePurchaseAsync(int id)
        {
            var purchase = await Repo.GetByIdAsync(id);
            if (purchase is null)
                return Error.NotFound("Purchase.NotFound", "Purchase not found");

            Repo.Delete(purchase);

            var saved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!saved)
                return Error.Faliure("Purchase.Failure", "Failed to delete purchase");

            return Result<bool>.Ok(true);
        }
    }
}
