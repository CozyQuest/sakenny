using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class RentedPropertyService : IRentedPropertyService
    {
        private readonly IBaseRepository<Renting> _rentingRepo;

        public RentedPropertyService(IBaseRepository<Renting> rentingRepo)
        {
            _rentingRepo = rentingRepo;
        }

        public async Task<IEnumerable<RentedPropertyDTO>> GetRentedPropertiesByUserAsync(string userId)
        {
            var rentings = await _rentingRepo.GetAllAsync(
                r => r.UserId == userId,
                null,
                r => r.Property,
                r => r.Property.Reviews
            );

            return rentings.Select(r =>
            {
                var userReview = r.Property?.Reviews?
                    .FirstOrDefault(review => review.UserId == userId && !review.IsDeleted);

                return new RentedPropertyDTO
                {
                    Title = r.Property?.Title,
                    MainImageUrl = r.Property?.MainImageUrl,
                    Rate = userReview?.Rate ?? 1, 
                    CheckIn = r.StartDate,
                    CheckOut = r.EndDate,
                    TotalPrice = r.TotalPrice
                };
            }).ToList();
        }
    }

}

