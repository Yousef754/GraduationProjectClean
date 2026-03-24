using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            //Count = count;
            Data = data;
        }
        //sdfs
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        //public int Count { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
