using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.ProductDTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Services.Abstraction
{
    public interface IProductService
    {
        // جلب كل المنتجات مع فلترة، ترتيب، صفحات
        Task<PaginatedResult<ProductReturnDto>> GetAllProductsAsync(ProductQueryParams queryParams);

        // جلب منتج واحد حسب الـ Id
        Task<ProductReturnDto?> GetProductByIdAsync(int id);

        // إنشاء منتج جديد (Admin فقط)
        Task<ProductReturnDto> CreateProductAsync(CreateProductDto dto);

        // تحديث منتج موجود (Admin فقط)
        Task<bool> UpdateProductAsync(int id, UpdateProductDto dto);

        // حذف منتج (Admin فقط)
        Task<bool> DeleteProductAsync(int id);

        Task<bool> UpdateProductImageAsync(int productId, string pictureUrl);
    }
}
