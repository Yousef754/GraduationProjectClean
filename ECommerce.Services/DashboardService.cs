using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Domain.Entities.ParchaseModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Domain.Entities.Sales;
using ECommerce.Services.Abstraction;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DashboardDto>> GetDashboardAsync()
        {
            // Employees
            var employees = await _unitOfWork
                .GetRepository<Employee, int>()
                .GetAllAsQueryable()
                .CountAsync();

            // Sales
            var sales = _unitOfWork
                .GetRepository<Sale, int>()
                .GetAllAsQueryable();

            var totalSalesAmount = await sales.SumAsync(s => s.TotalAmount);
            var completedSalesCount = await sales.CountAsync(s => s.Status == SaleStatus.Completed);
            var pendingSalesCount = await sales.CountAsync(s => s.Status == SaleStatus.Pending);

            // Purchases
            var purchases = _unitOfWork
                .GetRepository<Purchase, int>()
                .GetAllAsQueryable();

            var totalPurchasesAmount = await purchases.SumAsync(p => p.TotalAmount);
            var receivedOrdersCount = await purchases.CountAsync(p => p.Status == PurchaseStatus.ReceivedOrder);
            var pendingOrdersCount = await purchases.CountAsync(p => p.Status == PurchaseStatus.PendingOrder);

            // Products
            var products = _unitOfWork
                .GetRepository<Product, int>()
                .GetAllAsQueryable();

            var totalProducts = await products.CountAsync();
            var lowStockProducts = await products.CountAsync(p => p.Quantity <= 5);

            // Top Sales Employee
            var topSalesEmployee = await _unitOfWork
                .GetRepository<Sale, int>()
                .GetAllAsQueryable()
                .Include(s => s.Employee)
                .GroupBy(s => s.Employee.Name)
                .Select(g => new { EmployeeName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            // Top Purchases Employee
            var topPurchasesEmployee = await _unitOfWork
                .GetRepository<Purchase, int>()
                .GetAllAsQueryable()
                .Include(p => p.Employee)
                .GroupBy(p => p.Employee.Name)
                .Select(g => new { EmployeeName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            var dashboard = new DashboardDto
            {
                TotalEmployees = employees,
                TotalSalesAmount = totalSalesAmount,
                CompletedSalesCount = completedSalesCount,
                PendingSalesCount = pendingSalesCount,
                TotalPurchasesAmount = totalPurchasesAmount,
                ReceivedOrdersCount = receivedOrdersCount,
                PendingOrdersCount = pendingOrdersCount,
                TotalProducts = totalProducts,
                LowStockProducts = lowStockProducts,
                TopSalesEmployee = topSalesEmployee?.EmployeeName,
                //TopPurchasesEmployee = topPurchasesEmployee?.EmployeeName,
            };

            return Result<DashboardDto>.Ok(dashboard);
        }
    }
}
