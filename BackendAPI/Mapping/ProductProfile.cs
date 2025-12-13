using AutoMapper;
using BackendAPI.Dtos;
using BackendAPI.Models;

namespace BackendAPI.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
        CreateMap<Product,ProductResponseDto>();
    }

}
