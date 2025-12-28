using AutoMapper;
using BackendAPI.Dtos.Sales;
using SalesEntity = BackendAPI.Models.SalesOrder;

namespace BackendAPI.Mapping;

public class SalesProfile : Profile
{
    public SalesProfile()
    {
       CreateMap<SalesEntity, SalesResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SalesOrderId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : "Unknown" 
            ));
    }

}
