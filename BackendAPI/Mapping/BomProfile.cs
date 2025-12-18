using AutoMapper;
using BackendAPI.Dtos.Bom;
using BackendAPI.Models;

namespace BackendAPI.Mapping
{
    public class BomProfile : Profile
    {
        public BomProfile()
        {
            CreateMap<Bom, BomItemDto>() 
                .ForMember(dest => dest.RawMaterialName, opt => opt.MapFrom(src => src.RawMaterial != null ? src.RawMaterial.Name : null))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.RawMaterial != null ? src.RawMaterial.SKU : null));
        }
    }
}