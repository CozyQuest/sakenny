using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using Stripe;

namespace sakenny.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _context;
        private readonly IImageService _imageService;
        private readonly IReviewService _reviewService;

        public PropertyService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IReviewService reviewService,ApplicationDBContext context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _reviewService = reviewService;
            _context = context;
        }

        public async Task<PropertyDTO> AddPropertyAsync(AddPropertyDTO model, string Id)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), "Property model cannot be null");
            try
            {
                // Upload images first (external operations that can't be rolled back)
                var MainImageUrl = await _imageService.UploadImageAsync(model.MainImage);
                var imagesUrl = await _imageService.UploadImagesAsync(model.Images);

                // Create property entity
                var property = _mapper.Map<Property>(model);
                property.UserId = Id;
                property.MainImageUrl = MainImageUrl; // Set main image URL directly

                if (model.ServiceIds != null && model.ServiceIds.Any())
                {
                    foreach (var serviceId in model.ServiceIds)
                    {
                        var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
                        if (service != null)
                            property.Services.Add(service);
                    }
                }

                // Create image entities and set up relationships
                List<Image> images = new List<Image>
                {
                    new Image{
                        Url = MainImageUrl,
                        Property = property
                    }
                };

                foreach (var imageUrl in imagesUrl)
                {
                    images.Add(new Image
                    {
                        Url = imageUrl,
                        Property = property
                    });
                }

                // Set up property-image relationships (no circular dependency!)
                property.Images = images;

                // Create property permit
                var permit = new PropertyPermit
                {
                    Property = property,
                };

                // Create property snapshot
                var snapshot = _mapper.Map<PropertySnapshot>(property);
                snapshot.Property = property;
                snapshot.PropertyPermit = permit;
                snapshot.CreatedAt = DateTime.UtcNow;

                // Set up bidirectional relationships
                permit.PropertySnapshot = snapshot;
                property.PropertySnapshots.Add(snapshot);
                property.PropertyPermits.Add(permit);

                // Add all entities to context (no saving yet)
                await _unitOfWork.Properties.AddAsync(property);
                foreach (var image in images)
                {
                    await _unitOfWork.Images.AddAsync(image);
                }
                await _unitOfWork.PropertyPermits.AddAsync(permit);
                await _unitOfWork.PropertySnapshots.AddAsync(snapshot);

                // Single save at the end - all or nothing
                await _unitOfWork.SaveChangesAsync();

                // Convert to DTO and return
                var propertyDto = _mapper.Map<PropertyDTO>(property);
                return propertyDto;
            }
            catch (System.Exception)
            {
                // If any error occurs, nothing is saved to database
                throw;
            }
        }

        public async Task<List<PropertyDTO>> GetFilteredPropertiesAsync(PropertyFilterDTO filter)
        {
            var query = _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.Services)
                .Include(p => p.Images)
                .AsQueryable();

            // ? Apply filters
            if (filter.PropertyTypeIds?.Any() == true)
                query = query.Where(p => filter.PropertyTypeIds.Contains(p.PropertyTypeId));

            if (filter.ServiceIds?.Any() == true)
                query = query.Where(p => p.Services.Any(s => filter.ServiceIds.Contains(s.Id)));

            if (!string.IsNullOrEmpty(filter.Country))
                query = query.Where(p => p.Country == filter.Country);

            if (!string.IsNullOrEmpty(filter.City))
                query = query.Where(p => p.City == filter.City);

            if (!string.IsNullOrEmpty(filter.District))
                query = query.Where(p => p.District == filter.District);

            if (filter.MinPeople.HasValue)
                query = query.Where(p => p.PeopleCapacity >= filter.MinPeople);

            if (filter.MinSpace.HasValue)
                query = query.Where(p => p.Space >= filter.MinSpace);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            // ? Ordering
            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy.ToLower())
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "space_asc":
                        query = query.OrderBy(p => p.Space);
                        break;
                    case "space_desc":
                        query = query.OrderByDescending(p => p.Space);
                        break;
                        // Add more cases for rating, review count, etc.
                }
            }

            var properties = await query.ToListAsync();
            return _mapper.Map<List<PropertyDTO>>(properties);
        }

        public async Task<PropertyDTO> UpdatePropertyAsync(int id, UpdatePropertyDTO model, string userId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            if (property.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this property.");

            var existingMainImageUrl = property.MainImageUrl;

            _mapper.Map(model, property);

            if (model.MainImage != null)
            {
                var newMainImageUrl = await _imageService.UploadImageAsync(model.MainImage);
                property.MainImageUrl = newMainImageUrl;

                await _unitOfWork.Images.AddAsync(new Image
                {
                    Url = newMainImageUrl,
                    PropertyId = property.Id
                });
            }
            else
            {
                property.MainImageUrl = existingMainImageUrl; 
            }

            if (model.Images != null && model.Images.Any())
            {
                var imageUrls = await _imageService.UploadImagesAsync(model.Images);

                foreach (var imageUrl in imageUrls)
                {
                    var imageEntity = new Image
                    {
                        Url = imageUrl,
                        PropertyId = property.Id
                    };
                    property.Images ??= new List<Image>();
                    property.Images.Add(imageEntity);
                    await _unitOfWork.Images.AddAsync(imageEntity);
                }
            }

            var snapshot = _mapper.Map<PropertySnapshot>(property);
            snapshot.PropertyId = property.Id;
            snapshot.CreatedAt = DateTime.UtcNow;

            var permit = new PropertyPermit
            {
                PropertyID = property.Id,
                PropertySnapshot = snapshot
            };

            var images = await _unitOfWork.Images.GetAllAsync(img => img.PropertyId == property.Id);
            property.Images = images.ToList();


            snapshot.PropertyPermit = permit;

            property.PropertySnapshots.Add(snapshot);
            property.PropertyPermits.Add(permit);

            await _unitOfWork.PropertySnapshots.AddAsync(snapshot);
            await _unitOfWork.PropertyPermits.AddAsync(permit);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<PropertyDTO> GetPropertyDetailsAsync(int id)
        {
            var includes = new Expression<Func<Property, object>>[]
            {
               p => p.Images,
               p => p.PropertyType,
               p => p.Services,
               p => p.User
            };

            var property = await _unitOfWork.Properties.GetByIdAsync(id, includes);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<List<OwnedPropertyDTO>> GetTopRatedPropertiesAsync()
        {
            var includes = new Expression<Func<Property, object>>[]
            {
              p => p.Images
            };

            var properties = await _unitOfWork.Properties.GetAllAsync(
                p => !p.IsDeleted,
                includes: includes
            );

            var topRated = new List<OwnedPropertyDTO>();

            foreach (var property in properties)
            {
                var averageRating = await _reviewService.GetAverageRatingForPropertyAsync(property.Id);

                var dto = new OwnedPropertyDTO
                {
                    Id = property.Id,
                    Title = property.Title,
                    MainImageUrl = property.MainImageUrl,
                    PeopleCapacity = property.PeopleCapacity,
                    Space = property.Space,
                    RoomCount = property.RoomCount,
                    BathroomCount = property.BathroomCount,
                    Price = property.Price,
                    AverageRating = averageRating
                };

                topRated.Add(dto);
            }

            return topRated
                .OrderByDescending(p => p.AverageRating)
                .ThenByDescending(p => p.Price) 
                .Take(10)
                .ToList();
        }



        public async Task<IEnumerable<OwnedPropertyDTO>> GetUserOwnedPropertiesAsync(string userId)
        {

            var properties = await _unitOfWork.Properties.GetAllAsync(
                p => p.UserId == userId && !p.IsDeleted,
                includes: p => p.Images
            );

            if (properties == null || !properties.Any())
                return new List<OwnedPropertyDTO>();

            var result = new List<OwnedPropertyDTO>();

            foreach (var prop in properties)
            {
                var avgRating = await _reviewService.GetAverageRatingForPropertyAsync(prop.Id);

                result.Add(new OwnedPropertyDTO
                {
                    Id = prop.Id,
                    Title = prop.Title,
                    MainImageUrl = prop.MainImageUrl,
                    PeopleCapacity = prop.PeopleCapacity,
                    Space = prop.Space,
                    RoomCount = prop.RoomCount,
                    BathroomCount = prop.BathroomCount,
                    Price = prop.Price,
                    AverageRating = avgRating
                });
            }

            return result;
        }



    }
}