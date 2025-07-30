using AutoMapper;
using sakenny.Application.DTO;
using sakenny.DAL.Models;

namespace sakenny.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service, AddServiceDTO>().ReverseMap();
            CreateMap<Service, AddServiceDTO>()
                .ReverseMap();
            CreateMap<Service, UpdateServiceDTO>().ReverseMap();
            CreateMap<Service, GetAllServiceDTO>();

            CreateMap<PropertyType, AddTypeDTO>().ReverseMap();
            CreateMap<PropertyType, UpdateTypeDTO>().ReverseMap();
            CreateMap<PropertyType, GetAllTypeDTO>();


            CreateMap<Property, PropertyCheckoutDTO>()
                .ForMember(dest => dest.MainImageURL,
                           opt => opt.MapFrom(src => src.MainImageUrl))
                .ForMember(dest => dest.RentedDates,
                           opt => opt.MapFrom(src =>
                               src.Rentings
                                  .SelectMany(r => Enumerable.Range(0, (r.EndDate - r.StartDate).Days + 1)
                                                             .Select(offset => r.StartDate.AddDays(offset)))
                                  .Distinct()
                                  .ToList()));

            // Add User mapping
            CreateMap<RegistrationDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<Property, PropertyDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(img => img.Url).ToList() : new List<string>()));
            CreateMap<AddPropertyDTO, Property>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore());
            CreateMap<PropertySnapshot, Property>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Property, PropertySnapshot>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyPermitId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyPermit, opt => opt.Ignore())
                .ForMember(dest => dest.Property, opt => opt.Ignore())
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.MainImageUrl));

        }
    }
}
