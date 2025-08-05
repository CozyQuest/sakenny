using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.DTO.sakenny.DAL.DTOs;
using sakenny.DAL.Models;

namespace sakenny.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service, AddServiceDTO>()
                .ReverseMap();
            CreateMap<Service, UpdateServiceDTO>().ReverseMap();
            CreateMap<Service, GetAllServiceDTO>();

            CreateMap<PropertyType, AddTypeDTO>().ReverseMap();
            CreateMap<PropertyType, UpdateTypeDTO>().ReverseMap();
            CreateMap<PropertyType, GetAllTypeDTO>();
            CreateMap<User, UserOwnerDTO>();


            CreateMap<User, UserProfileDTO>()
            .ForMember(dest => dest.Fname, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.Lname, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.UrlProfilePicture));

            CreateMap<User, UserPublicProfileDTO>()
             .ForMember(dest => dest.Fname, opt => opt.MapFrom(src => src.FirstName))
             .ForMember(dest => dest.Lname, opt => opt.MapFrom(src => src.LastName))
             .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.UrlProfilePicture));

            CreateMap<Property, PropertyCheckoutDTO>()
                .ForMember(dest => dest.MainImageURL,
                           opt => opt.MapFrom(src => src.MainImageUrl))
                .ForMember(dest => dest.RentedDates,
                           opt => opt.MapFrom(src =>
                               src.Rentings
                                  .SelectMany(r => Enumerable.Range(0, (r.EndDate - r.StartDate).Days + 1)
                                                             .Select(offset => r.StartDate.AddDays(offset)))
                                  .Distinct()
                                  .ToList()))
                .ForMember(dest => dest.Rating,
                           opt => opt.MapFrom(src =>
                               src.Reviews.Any() ? (int)Math.Round(src.Reviews.Average(r => r.Rate)) : 0))
                .ForMember(dest => dest.RatingCount,
                           opt => opt.MapFrom(src => src.Reviews.Count));


            // Add User mapping
            CreateMap<RegistrationDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<Property, GetAllPropertiesDTO>();

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


            CreateMap<UpdatePropertyDTO, Property>()
              .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore())
              .ForMember(dest => dest.Images, opt => opt.Ignore())
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                 srcMember != null && srcMember.ToString() != "0"));

            CreateMap<Property, PropertySnapshot>()
               .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<PostReviewDTO, Review>();

            CreateMap<UserHostRequestDTO, User>();

            CreateMap<User, UserHostRequestDTO>()
                .ForMember(dest => dest.Fname, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Lname, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.UrlProfilePicture))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));





        }
    }
}
