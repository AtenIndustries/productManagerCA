using AutoMapper;
using ProductManager.BAL.DTO;

namespace ProductManager.API.Contracts;
 
public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    { 
        CreateMap<ProductDataBody, ProductDataDTO>();
    }
}