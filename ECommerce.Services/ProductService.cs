using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Persistence;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Exceptions;
using ECommerce.Services.Specifications.ProductSpecifications;
using ECommerce.Shared;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.ProductDTOs;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // جلب المنتجات مع Pagination + Filtering + Sorting
        public async Task<PaginatedResult<ProductReturnDto>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            // 1️⃣ انشأ Specification للفلترة + Pagination + Sorting
            var spec = new ProductWithTypeAndBrandSpecification(queryParams);

            // 2️⃣ جلب الـ Repository
            var repo = _unitOfWork.GetRepository<Product, int>();

            // 3️⃣ جلب IQueryable مباشرة من Repository
            var queryable = repo.GetAllAsQueryable();

            // 4️⃣ تطبيق الـ Specification
            var query = SpecificationEvaluator.CreateQuery<Product, int>(queryable, spec);

            // 5️⃣ احسب العدد الكلي قبل Pagination
            //var totalCount = await query.CountAsync();

            // 6️⃣ جلب العناصر بعد Pagination
            var products = await query.ToListAsync();

            // 7️⃣ Mapping من Entity → DTO
            var data = _mapper.Map<IEnumerable<ProductReturnDto>>(products);

            // 8️⃣ ارجع PaginatedResult
            return new PaginatedResult<ProductReturnDto>(
                queryParams.PageIndex,
                queryParams.PageSize,
                
                data
            );
        }

        // جلب منتج واحد حسب Id
        public async Task<ProductReturnDto?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithTypeAndBrandSpecification(id);
            var repo = _unitOfWork.GetRepository<Product, int>();

            var product = await repo.GetByIdAsync(spec);
            if (product == null) return null;

            return _mapper.Map<ProductReturnDto>(product);
        }

        // إنشاء منتج جديد
        public async Task<ProductReturnDto> CreateProductAsync(CreateProductDto dto)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = _mapper.Map<Product>(dto);

            await repo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductReturnDto>(product);
        }

        // تحديث منتج موجود
        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            // جلب الـ entity من DbContext عبر Repository
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                return false;

            // Partial update: نغير بس الحقول اللي موجودة في DTO
            if (dto.Name != null)
                product.Name = dto.Name;

            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (dto.Quantity.HasValue)
                product.Quantity = dto.Quantity.Value;

            // إذا كان الـ entity detached، استخدم Update
            // إذا جاي من GetByIdAsync من نفس DbContext، مش محتاج
            // repo.Update(product); // ← ممكن تشيلها

            // احفظ التغييرات في DbContext
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // حذف منتج
        public async Task<bool> DeleteProductAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);
            if (product == null) return false;

            repo.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateProductImageAsync(int productId, string pictureUrl)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(productId);

            if (product == null)
                return false;

            product.PictureUrl = pictureUrl;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }


    }
}
