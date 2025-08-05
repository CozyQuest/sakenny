using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace sakenny.Application.DTO
{
    public class UpdatePropertyDTO
    {

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? PropertyTypeId { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public int? BuildingNo { get; set; }

        public int? Level { get; set; }

        public int? FlatNo { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public int? RoomCount { get; set; }

        public int? BathroomCount { get; set; }

        public double? Space { get; set; }

        public decimal? Price { get; set; }

        public int? PeopleCapacity { get; set; }

        [FromForm(Name = "mainImage")]
        public IFormFile? MainImage { get; set; }

        [FromForm(Name = "images")]
        public List<IFormFile>? Images { get; set; }

        [FromForm(Name = "ServiceIds")]
        public HashSet<int>? ServiceIds { get; set; }

    }
}

