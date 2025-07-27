using AutoMapper;
using sakenny.Application.DTO;
using sakenny.DAL.Models;

namespace sakenny.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service,AddServiceDTO>()
                .ReverseMap();
            CreateMap<Service, UpdateServiceDTO>().ReverseMap();
            CreateMap<Service, DeleteServiceDTO>()
                .ReverseMap();
        }
    }
}
