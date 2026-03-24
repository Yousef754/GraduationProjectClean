using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.ProductDTOs
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } 
        
        public string? Description { get; set; } 

        public string? Color { get; set; } 

        public string? PictureUrl { get; set; } 

        public decimal? Price { get; set; }

        public int? CategoryId { get; set; }

        public int? Quantity { get; set; }   

    }
}
