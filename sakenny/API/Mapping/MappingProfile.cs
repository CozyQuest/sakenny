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
            .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.UrlProfilePicture))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<User, UserPublicProfileDTO>()
             .ForMember(dest => dest.Fname, opt => opt.MapFrom(src => src.FirstName))
             .ForMember(dest => dest.Lname, opt => opt.MapFrom(src => src.LastName))
             .ForMember(dest => dest.ProfilePicUrl, opt => opt.MapFrom(src => src.UrlProfilePicture))
             .ForMember(dest => dest.Role, opt => opt.Ignore());

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

            CreateMap<GetAllPropertiesDTO, Property>();
            CreateMap<PropertyFilterDTO, Property>();

            CreateMap<Property, GetAllPropertiesDTO>()
    .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src =>
        src.Images != null && src.Images.Any() ? src.Images.First().Url : string.Empty))
    .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
        src.Images != null ? src.Images.Select(img => img.Url).ToList() : new List<string>()))
    .ForMember(dest => dest.PropertyTypeName, opt => opt.MapFrom(src =>
        src.PropertyType != null ? src.PropertyType.Name : string.Empty))
    .ForMember(dest => dest.ServiceNames, opt => opt.MapFrom(src =>
        src.Services != null ? src.Services.Select(s => s.Name).ToList() : new List<string>()))
    .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
        src.Reviews != null && src.Reviews.Any() ? (double?)Math.Round(src.Reviews.Average(r => r.Rate), 2) : null))
    .ForMember(dest => dest.ReviewsCount, opt => opt.MapFrom(src =>
        src.Reviews != null ? src.Reviews.Count : 0));

            CreateMap<UserProfileDTO, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Fname))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Lname))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlProfilePicture, opt => opt.MapFrom(src => src.ProfilePicUrl)).ReverseMap();

            CreateMap<Property, HostedPropertyDTO>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.MainImageUrl))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Space))
                .ForMember(dest => dest.Beds, opt => opt.MapFrom(src => src.RoomCount))
                .ForMember(dest => dest.Baths, opt => opt.MapFrom(src => src.BathroomCount))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 
                    src.Reviews != null && src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => r.Rate), 2) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => 
                    src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => $"{src.City}, {src.Country}"));



        }
    }
}
