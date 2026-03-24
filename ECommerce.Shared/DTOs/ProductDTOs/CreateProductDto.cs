using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.ProductDTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string Color { get; set; } = default!;

        public string PictureUrl { get; set; } = default!;

        public decimal Price { get; set; }
        public int CategoryId { get; set; }

        public int Quantity { get; set; }   
    }
}
