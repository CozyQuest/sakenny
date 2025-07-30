using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace sakenny.Application.DTO
{
    public class AddPropertyDTO
    {
        [Required, MaxLength(50), MinLength(3), RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Title can only contain letters, numbers, and spaces.")]
        public string Title { get; set; }

        [Required, MaxLength(500), MinLength(10)]
        [RegularExpression(
            @"^(?!.*\b(?:\+?\d[\d\- ]{7,}\d|[\w\.\-]+@[\w\.\-]+\.\w{2,})\b)[\s\S]*$",
            ErrorMessage = "Description must not contain phone numbers or email addresses."
        )]
        public string Description { get; set; }

        [Required]
        public int PropertyTypeId { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string District { get; set; }
        [Required, Range(0, double.MaxValue, ErrorMessage = "Building number must be a positive number.")]
        public int BuildingNo { get; set; }
        [Required, Range(0, double.MaxValue, ErrorMessage = "Level must be a positive number.")]
        public int Level { get; set; }
        [Required, Range(0, double.MaxValue, ErrorMessage = "Flat number must be a positive number.")]
        public int FlatNo { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "RoomCount must be a positive number.")]
        public int RoomCount { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "BathroomCount must be a positive number.")]
        public int BathroomCount { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Area must be a positive number.")]
        public double Space { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "PeopleCapacity must be a positive number.")]
        public int PeopleCapacity { get; set; }

        [Required]
        public string MainImageUrl { get; set; }
        [FromForm(Name = "file")]
        public List<IFormFile> Images { get; set; }
    }
}
