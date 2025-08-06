using sakenny.DAL.Models;

namespace sakenny.Application.DTO
{
    namespace sakenny.DAL.DTOs
    {
        public class GetAllPropertiesDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string District { get; set; }
            public int BuildingNo { get; set; }
            public int? Level { get; set; }
            public int? FlatNo { get; set; }
            public decimal Longitude { get; set; }
            public decimal Latitude { get; set; }
            public int RoomCount { get; set; }
            public int BathroomCount { get; set; }
            public double Space { get; set; }
            public decimal Price { get; set; }
            public int PeopleCapacity { get; set; }
            public PropertyStatus Status { get; set; }
            public string MainImageUrl { get; set; }
            public string PropertyTypeName { get; set; }

            public List<string>? ImageUrls { get; set; }
            public double? AverageRating { get; set; }
            public int ReviewsCount { get; set; }
            public List<string>? ServiceNames { get; set; }
        }

    }

}
