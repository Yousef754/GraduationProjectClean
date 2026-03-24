using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.ProductDTOs
{
    public class ProductReturnDto
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string Color { get; set; } = default!;

        public string PictureUrl { get; set; } = default!;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = default!;

        public int Quantity { get; set; }   

    }
}
