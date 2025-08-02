using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddOrUpdateRatingAsync(int propertyId, PostRatingDTO dto, string userId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            var hasRented = await _unitOfWork.Rentings.GetAllAsync(r =>
                r.PropertyId == propertyId && r.UserId == userId);

            if (!hasRented.Any())
                throw new UnauthorizedAccessException("You must rent the property before rating.");

            var existingReview = (await _unitOfWork.Reviews.GetAllAsync(r =>
                r.PropertyId == propertyId && r.UserId == userId)).FirstOrDefault();

            if (existingReview != null)
            {
                existingReview.Rate = dto.Rate;
            }
            else
            {
                var newReview = new Review
                {
                    PropertyId = propertyId,
                    UserId = userId,
                    Rate = dto.Rate
                };
                await _unitOfWork.Reviews.AddAsync(newReview);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddReviewAsync(int propertyId, PostReviewDTO dto, string userId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            if (property.UserId == userId)
                throw new InvalidOperationException("You cannot review your own property.");

            var hasRented = await _unitOfWork.Rentings.GetAllAsync(r =>
                r.PropertyId == propertyId && r.UserId == userId);

            if (!hasRented.Any())
                throw new UnauthorizedAccessException("You must rent the property before reviewing.");

            var existingReview = (await _unitOfWork.Reviews.GetAllAsync(r =>
                r.PropertyId == propertyId && r.UserId == userId)).FirstOrDefault();

            if (existingReview != null)
            {
                if (!string.IsNullOrEmpty(existingReview.ReviewText))
                    throw new InvalidOperationException("You have already submitted a review text and cannot change it.");

                bool hasExistingRate = existingReview.Rate >= 1 && existingReview.Rate <= 5;
                bool hasRateInRequest = dto.Rate.HasValue && dto.Rate.Value >= 1 && dto.Rate.Value <= 5;

                if (!hasExistingRate && !hasRateInRequest)
                    throw new InvalidOperationException("Rating is required if not already provided.");

                existingReview.ReviewText = dto.ReviewText;
                if (hasRateInRequest)
                    existingReview.Rate = dto.Rate.Value;
            }
            else
            {
                if (!dto.Rate.HasValue || dto.Rate.Value < 1 || dto.Rate.Value > 5)
                    throw new InvalidOperationException("Rating is required and must be between 1 and 5.");

                var newReview = new Review
                {
                    PropertyId = propertyId,
                    UserId = userId,
                    ReviewText = dto.ReviewText,
                    Rate = dto.Rate.Value
                };
                await _unitOfWork.Reviews.AddAsync(newReview);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsForPropertyAsync(int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            var reviews = await _unitOfWork.Reviews.GetAllAsync(
                r => r.PropertyId == propertyId && !r.IsDeleted,
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                r => r.User
            );

            return reviews.Select(r => new ReviewDTO
            {
                ReviewText = r.ReviewText,
                Rate = r.Rate,
                CreatedAt = r.CreatedAt,
                UserFullName = $"{r.User.FirstName} {r.User.LastName}",
                UserProfilePicUrl = r.User.UrlProfilePicture
            }).ToList();
        }
    }

}

