using AutoMapper;
using BackendAPI.Dtos.Bom;
using BackendAPI.Models;

namespace BackendAPI.Mapping
{
    public class BomProfile : Profile
    {
        public BomProfile()
        {
            // Read: Entity -> Response DTO
            CreateMap<Bom, BomItemDto>()
            .ForMember(d => d.RawMaterialName,
                opt => opt.MapFrom(s => s.RawMaterial!.Name));

        }
    }
}