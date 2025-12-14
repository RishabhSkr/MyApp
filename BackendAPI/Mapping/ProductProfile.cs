using AutoMapper;
using BackendAPI.Dtos.Product;
using BackendAPI.Models;

namespace BackendAPI.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
        CreateMap<Product,ProductResponseDto>()
            .ForMember(dest => dest.Id,opt => opt.MapFrom(src => src.ProductID));
    }

}
