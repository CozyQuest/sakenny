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

        private readonly IImageService _imageService;
        private readonly IReviewService _reviewService;

        public PropertyService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IReviewService reviewService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _reviewService = reviewService;
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

        public async Task<PropertyDTO> UpdatePropertyAsync(int id, UpdatePropertyDTO model, string userId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id, p => p.Services, p => p.Images);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            if (property.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this property.");

            _mapper.Map(model, property);

            if (model.ServiceIds != null)
            {
                var existingServices = property.Services.ToList();

                foreach (var existingService in existingServices)
                {
                    property.Services.Remove(existingService);
                }

                foreach (var serviceId in model.ServiceIds)
                {
                    var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
                    if (service != null && !service.IsDeleted)
                    {
                        property.Services.Add(service);
                    }
                }
            }

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

            if (model.Images != null && model.Images.Any())
            {
                var oldImages = await _unitOfWork.Images.GetAllAsync(img => img.PropertyId == property.Id);

                foreach (var oldImage in oldImages)
                {
                    _unitOfWork.Images.DeleteAsync(oldImage);
                }

                property.Images.Clear();

                var newImageUrls = await _imageService.UploadImagesAsync(model.Images);
                foreach (var imageUrl in newImageUrls)
                {
                    var newImage = new Image
                    {
                        Url = imageUrl,
                        PropertyId = property.Id
                    };
                    property.Images.Add(newImage);
                    await _unitOfWork.Images.AddAsync(newImage);
                }
            }

            var updatedImages = await _unitOfWork.Images.GetAllAsync(img => img.PropertyId == property.Id);
            property.Images = updatedImages.ToList();

            var snapshot = _mapper.Map<PropertySnapshot>(property);
            snapshot.PropertyId = property.Id;
            snapshot.CreatedAt = DateTime.UtcNow;

            var permit = new PropertyPermit
            {
                PropertyID = property.Id,
                PropertySnapshot = snapshot
            };

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
               p => p.PropertyType
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