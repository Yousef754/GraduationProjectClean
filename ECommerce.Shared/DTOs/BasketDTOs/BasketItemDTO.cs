using System.ComponentModel.DataAnnotations;

namespace ECommerce.Shared.DTOs.BasketDTOs
{
    public record BasketItemDTO
(
    int ProductId,
    string? ProductName,
    string? PictureUrl,
    decimal Price,
    int Quantity
);
}
