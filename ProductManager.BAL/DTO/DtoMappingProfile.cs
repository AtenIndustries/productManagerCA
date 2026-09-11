using AutoMapper;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;
 
public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    { 
        CreateMap<ProductDTO, Product>(MemberList.None).ReverseMap(); //MemberList.None ignores non matching properties
        CreateMap<ProductDataDTO, Product>(MemberList.None).ReverseMap(); 
    }
}