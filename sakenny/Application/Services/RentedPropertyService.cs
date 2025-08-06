using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System.Linq.Expressions;

namespace sakenny.Application.Services
{
    public class RentedPropertyService : IRentedPropertyService
    {
        private readonly IBaseRepository<Renting> _rentingRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDBContext _context;

        public RentedPropertyService(IBaseRepository<Renting> rentingRepo, IUnitOfWork unitOfWork, ApplicationDBContext context)
        {
            _rentingRepo = rentingRepo;
            _unitOfWork = unitOfWork;
            _context = context;
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

        public async Task<HostEarningsDTO> GetHostEarningsAsync(string hostId)
        {
            var properties = await _unitOfWork.Properties.GetAllAsync(
                p => p.UserId == hostId && !p.IsDeleted,
                includes: new Expression<Func<Property, object>>[]
                {
                  p => p.Rentings!,
                  p => p.Reviews!,
                  p => p.Images!
                }
            );

            var filteredProperties = properties
                .Where(p => p.Rentings != null && p.Rentings.Any())
                .ToList();

            foreach (var property in filteredProperties)
            {
                foreach (var renting in property.Rentings!)
                {
                    renting.User = (User) await _unitOfWork.userManager.FindByIdAsync(renting.UserId);
                }
            }

            var earningsItems = new List<EarningsItemDTO>();
            decimal totalProfit = 0;

            foreach (var property in filteredProperties)
            {
                var mainImageUrl = property.MainImageUrl;

                foreach (var renting in property.Rentings!)
                {
                    var renter = renting.User;
                    var review = property.Reviews?.FirstOrDefault(r => r.UserId == renter?.Id);

                    earningsItems.Add(new EarningsItemDTO
                    {
                        PropertyTitle = property.Title,
                        PropertyImageUrl = mainImageUrl,
                        RenterFullName = renter != null
                            ? $"{renter.FirstName} {renter.LastName}"
                            : "Unknown",
                        CheckIn = renting.StartDate,
                        CheckOut = renting.EndDate,
                        TransactionDate = renting.TransactionDate,
                        Rating = review?.Rate,
                        TotalPrice = renting.TotalPrice
                    });

                    totalProfit += renting.TotalPrice;
                }
            }

            var sortedItems = earningsItems
                .OrderByDescending(e => e.TransactionDate)
                .ToList();

            return new HostEarningsDTO
            {
                TotalProfit = totalProfit,
                Items = sortedItems
            };
        }


    }
}


