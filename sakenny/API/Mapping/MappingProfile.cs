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
            CreateMap<Property, PropertyCheckoutDTO>()
                .ForMember(dest => dest.MainImageURL,
                           opt => opt.MapFrom(src => src.MainImage.Url))
                .ForMember(dest => dest.RentedDates,
                           opt => opt.MapFrom(src =>
                               src.Rentings
                                  .SelectMany(r => Enumerable.Range(0, (r.EndDate - r.StartDate).Days + 1)
                                                             .Select(offset => r.StartDate.AddDays(offset)))
                                  .Distinct()
                                  .ToList()));
        }
    }
}
