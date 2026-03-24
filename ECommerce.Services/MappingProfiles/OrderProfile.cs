using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // Mapping for addresses
            CreateMap<AddressDTO, OrderAddress>().ReverseMap();

            // Mapping for orders
            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(dest => dest.ShipToAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod));

            // Mapping for order items
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom<OrderItemPictureUrlResolver>());

            // Mapping for delivery methods
            CreateMap<DeliveryMethod, DeliveryMethodDTO>();
        }
    }
}
