using AutoMapper;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;
 
public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    { 
        CreateMap<ProductReadDTO, Product>(MemberList.None).ReverseMap(); //MemberList.None ignores non matching properties
        CreateMap<ProductWriteDTO, Product>(MemberList.None).ReverseMap(); 
    }
}