using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using sakenny.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sakenny.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class dummyController : ControllerBase
    {
        ApplicationDBContext db;
        private readonly IUnitOfWork _unitOfWork;

        public dummyController(ApplicationDBContext db, IUnitOfWork unitOfWork)
        {
            this.db = db;
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        public IActionResult Post([FromBody] string _description)
        {
            
            var dum = new DummyTable
            {
                description = _description,
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
             };
            db.Add(dum);
            db.SaveChanges();
            if (string.IsNullOrEmpty(_description))
            {
                return BadRequest("Description cannot be null or empty.");
            }
            // Simulate saving the description
            // SaveDescriptionToDatabase(description);
            return Ok("Description submitted successfully.");
        }
        //[HttpPost("/test")]
        //public async Task<IActionResult> test(int id)
        //{
        //    Property property =new Property
        //    {
        //        Title = "The most amazing apartment",
        //        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
        //        Price = 1000.00m,
        //        RoomCount = 3,
        //        BathroomCount = 2,
        //        Space = 120.5,
        //        PeopleCapacity = 6,
        //        Country = "Egypt",
        //        City = "Alexandria",
        //        District = "Loran",
        //        BuildingNo = 1,
        //        Level = 1,
        //        FlatNo = 101,
        //        Longitude = 0.0m,
        //        Latitude = 0.0m,
        //        UserId = "28fc157b-d6d6-4903-9373-ce39a830ba6d",
        //        //UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        //        PropertyType = new PropertyType
        //        {
        //            Name = "Apartment"
        //        }
        //    };
        //    Image image1 = new Image
        //    {
        //        Url = "https://shreethemes.in/hously/landing/assets/images/property/1.jpg",
        //        Property = property
        //    };
        //    Image image2 = new Image
        //    {
        //        Url = "https://shreethemes.in/hously/landing/assets/images/property/2.jpg",
        //        Property = property
        //    };
        //    Service service1 = new Service
        //    {
        //        Name = "Free Wi-Fi"
        //    };
        //    service1.Properties.Add(property);
        //    Service service2 = new Service
        //    {
        //        Name = "Free Parking"
        //    };
        //    service2.Properties.Add(property);
        //    Review review1 = new Review
        //    {
        //        Rate = 5,
        //        ReviewText = "This is a great property!",
        //        UserId = "28fc157b-d6d6-4903-9373-ce39a830ba6d",
        //        Property = property
        //    };
        //    Review review2 = new Review
        //    {
        //        Rate = 4,
        //        ReviewText = "I had a wonderful stay here.",
        //        UserId = "28fc157b-d6d6-4903-9373-ce39a830ba6d",
        //        Property = property
        //    };
        //    Renting renting1 = new Renting
        //    {
        //        Property = property,
        //        UserId = "28fc157b-d6d6-4903-9373-ce39a830ba6d",
        //        StartDate = DateTime.UtcNow.AddDays(2),
        //        EndDate = DateTime.UtcNow.AddDays(7),
        //        TotalPrice = 7000.00m
        //    };
        //    Renting renting2 = new Renting
        //    {
        //        Property = property,
        //        UserId = "28fc157b-d6d6-4903-9373-ce39a830ba6d",
        //        StartDate = DateTime.UtcNow.AddDays(10),
        //        EndDate = DateTime.UtcNow.AddDays(15),
        //        TotalPrice = 5000.00m
        //    };

        //    await _unitOfWork.Properties.AddAsync(property);

        //    await _unitOfWork.Images.AddAsync(image1);
        //    await _unitOfWork.Images.AddAsync(image2);

        //    await _unitOfWork.Services.AddAsync(service1);
        //    await _unitOfWork.Services.AddAsync(service2);

        //    await _unitOfWork.Reviews.AddAsync(review1);
        //    await _unitOfWork.Reviews.AddAsync(review2);

        //    await _unitOfWork.Rentings.AddAsync(renting1);
        //    await _unitOfWork.Rentings.AddAsync(renting2);

        //    await _unitOfWork.SaveChangesAsync();

        //    property.MainImageId = image1.Id;

        //    await _unitOfWork.SaveChangesAsync();
        //    // return a property without the navigation properties to avoid circular reference issues
        //    return Ok(new
        //    {
        //        property.Id,
        //        property.Title,
        //        property.Description,
        //        property.Price,
        //        property.RoomCount,
        //        property.BathroomCount,
        //        property.Space,
        //        property.PeopleCapacity,
        //        property.Country,
        //        property.City,
        //        property.District,
        //        property.BuildingNo,
        //        property.Level,
        //        property.FlatNo,
        //        property.Longitude,
        //        property.Latitude,
        //        property.UserId,
        //        MainImage = new { property.MainImage.Url },
        //        property.MainImageId,
        //        PropertyType = new
        //        {
        //            property.PropertyType.Id,
        //            property.PropertyType.Name
        //        },
        //        // list of images
        //        Images = property.Images.Select(i => new { i.Id, i.Url }).ToList(),
        //        // list of services
        //        Services = property.Services.Select(s => new { s.Id, s.Name }).ToList(),
        //        // list of reviews
        //        Reviews = property.Reviews.Select(r => new
        //        {
        //            r.Id,
        //            r.Rate,
        //            r.ReviewText,
        //            User = new { r.UserId } // Assuming you want to return the UserId only
        //        }).ToList(),
        //        // list of rentings
        //        Rentings = property.Rentings.Select(r => new
        //        {
        //            r.Id,
        //            r.StartDate,
        //            r.EndDate,
        //            r.TotalPrice,
        //            UserId = r.UserId // Assuming you want to return the UserId only
        //        }).ToList()
        //    });
        //}
    }
}
