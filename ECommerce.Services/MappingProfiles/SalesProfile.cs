using AutoMapper;
using ECommerce.Domain.Entities.Sales;
using ECommerce.Shared.DTOs.SalesDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.MappingProfiles
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            CreateMap<Sale, SaleToReturnDto>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Product.Category.Name))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<CreateSaleDto, Sale>();
        }
    }
}
