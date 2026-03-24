using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared
{
    public class ProductQueryParams
    {
        // فلترة حسب Category بدل brand/type
        public int? CategoryId { get; set; }

        // البحث على الاسم أو الوصف
        public string? Search { get; set; }

        // طريقة ترتيب النتائج
        public ProductSortingOptions Sort { get; set; }

        // Pagination
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = (value <= 0) ? 1 : value;
        }

        private const int _defaultPageSize = 5;
        private const int _maxPageSize = 10;
        private int _pageSize = _defaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                    _pageSize = _defaultPageSize;
                else if (value > _maxPageSize)
                    _pageSize = _maxPageSize;
                else
                    _pageSize = value;
            }
        }
    }
}
