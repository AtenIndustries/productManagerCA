using AutoMapper;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<ProductReadDTO, Product>(MemberList.None)//MemberList.None ignores non matching properties
            .ReverseMap().ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => (src.ProductCategory != null) ? src.ProductCategory.CategoryName : string.Empty));
        CreateMap<ProductWriteDTO, Product>(MemberList.None).ReverseMap();
    }
}