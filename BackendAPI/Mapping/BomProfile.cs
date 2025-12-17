using AutoMapper;
using BackendAPI.Dtos.Bom;
using BackendAPI.Models;

namespace BackendAPI.Mapping
{
    public class BomProfile : Profile
    {
        public BomProfile()
        {
            /// 1. Single Item Mapping (Child)
            // Logic: Entity ke 'RawMaterial' object se 'Name' utha kar DTO ke 'RawMaterialName' me daalo
            CreateMap<Bom, BomResponseItemDto>()
                .ForMember(dest => dest.RawMaterialName, opt => opt.MapFrom(src => src.RawMaterial!=null ? src.RawMaterial.Name : null))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.RawMaterial!=null ? src.RawMaterial.SKU : null));

            // 2. Parent Mapping is thoda tricky (Group By Logic)
            // Kyunki Database 'Flat List' deta hai (Row 1, Row 2...), 
            // lekin Output 'Nested' hai (Product -> List<Items>).
            // Iska logic hum Service Layer mein manually likhenge kyunki yeh direct mapping nahi hai.

        }
    }
}