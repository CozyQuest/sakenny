using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sakenny.Application.DTO;
using sakenny.Application.DTO.sakenny.DAL.DTOs;
using sakenny.Application.Interfaces;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using System.Linq.Expressions;


namespace sakenny.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _context;
        private readonly IImageService _imageService;
        private readonly IReviewService _reviewService;

        public PropertyService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IReviewService reviewService, ApplicationDBContext context)
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
            filter ??= new PropertyFilterDTO(); // Ensure filter is not null

            // Start building query from Properties table with necessary Includes
            var query = _context.Properties
                .AsNoTracking() // No tracking since this is a read-only operation
                .Include(p => p.PropertyType)
                .Include(p => p.Services)
                .Include(p => p.Images)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Apply filters step-by-step
            if (filter.PropertyTypeIds?.Any() == true)
                query = query.Where(p => filter.PropertyTypeIds.Contains(p.PropertyTypeId));

            if (!string.IsNullOrEmpty(filter.Country))
                query = query.Where(p => p.Country == filter.Country);

            if (!string.IsNullOrEmpty(filter.City))
                query = query.Where(p => p.City == filter.City);

            if (!string.IsNullOrEmpty(filter.District))
                query = query.Where(p => p.District == filter.District);

            if (filter.MinPeople.HasValue && filter.MinPeople > 0)
                query = query.Where(p => p.PeopleCapacity >= filter.MinPeople);

            if (filter.MinSpace.HasValue && filter.MinSpace > 0)
                query = query.Where(p => p.Space >= filter.MinSpace);

            if (filter.MinPrice.HasValue && filter.MinPrice > 0)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue && filter.MaxPrice > 0)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            // Apply ordering
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
                    default:
                        query = query.OrderByDescending(p => p.Id); // fallback
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.Id); // fallback
            }

            // Execute query so far and get list
            var properties = await query.ToListAsync();

            // Apply service filtering in-memory
            if (filter.ServiceIds?.Any(id => id > 0) == true)
            {
                properties = properties
                    .Where(p => p.Services != null && p.Services.Any(s => filter.ServiceIds.Contains(s.Id)))
                    .ToList();
            }
            // Map result to DTO and return
            return _mapper.Map<List<PropertyDTO>>(properties);
        }

        public async Task<PropertyDTO> UpdatePropertyAsync(int id, UpdatePropertyDTO model, string userId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id, p => p.Services, p => p.Images);
            if (property == null || property.IsDeleted)
                throw new KeyNotFoundException("Property not found.");

            if (property.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this property.");

            _mapper.Map(model, property);

            // Handle services (unchanged)
            if (model.ServiceIds != null)
            {
                var services = await _unitOfWork.Services
                    .GetAllAsync(s => model.ServiceIds.Contains(s.Id) && !s.IsDeleted);
                property.Services.Clear();
                foreach (var service in services)
                {
                    property.Services.Add(service);
                }
            }

            // ✅ STEP 1: Handle image removals FIRST
            if (model.ImageUrlsToRemove != null && model.ImageUrlsToRemove.Any())
            {
                var imagesToRemove = property.Images
                    .Where(img => model.ImageUrlsToRemove.Contains(img.Url))
                    .ToList();
                    
                foreach (var imageToRemove in imagesToRemove)
                {
                    property.Images.Remove(imageToRemove);
                    // TODO: Delete the actual file from storage when IImageService.DeleteImageAsync is implemented
                    // This would prevent orphaned files in blob storage
                }
            }

            // ✅ STEP 2: Handle main image update (NEW LOGIC)
            if (model.MainImage != null)
            {
                // User uploaded a new main image
                var newMainImageUrl = await _imageService.UploadImageAsync(model.MainImage);
                property.MainImageUrl = newMainImageUrl;
                
                // Add to images collection if not already present
                if (!property.Images.Any(img => img.Url == newMainImageUrl))
                {
                    property.Images.Add(new Image
                    {
                        Url = newMainImageUrl,
                        PropertyId = property.Id
                    });
                }
            }
            else if (!string.IsNullOrEmpty(model.MainImageUrl))
            {
                // ✅ NEW: User selected an existing image as main
                // Verify the URL exists in the property's images
                if (property.Images.Any(img => img.Url == model.MainImageUrl))
                {
                    property.MainImageUrl = model.MainImageUrl;
                    Console.WriteLine($"✅ Set existing image as main: {model.MainImageUrl}");
                }
                else
                {
                    throw new ArgumentException($"The specified main image URL does not exist in this property's images: {model.MainImageUrl}");
                }
            }

            // ✅ STEP 3: Add new additional images (PRESERVE existing ones)
            if (model.Images != null && model.Images.Any())
            {
                var newImageUrls = await _imageService.UploadImagesAsync(model.Images);
                
                foreach (var imageUrl in newImageUrls)
                {
                    // Only add if not already present (prevent duplicates)
                    if (!property.Images.Any(img => img.Url == imageUrl))
                    {
                        property.Images.Add(new Image
                        {
                            Url = imageUrl,
                            PropertyId = property.Id
                        });
                    }
                }
            }

            // Rest of the method unchanged (snapshot creation, etc.)
            var permit = new PropertyPermit
            {
                PropertyID = property.Id,
                status = PropertyStatus.Pending
            };

            var snapshot = _mapper.Map<PropertySnapshot>(property);
            snapshot.PropertyId = property.Id;
            snapshot.CreatedAt = DateTime.UtcNow;
            snapshot.PropertyPermit = permit;
            
            permit.PropertySnapshot = snapshot;

            property.PropertySnapshots.Add(snapshot);
            property.PropertyPermits.Add(permit);

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
        public async Task<IEnumerable<HostedPropertyDTO>> GetUserOwnedPropertiesPagedAsync(string userId, int PageNumber, int PageSize)
        {

            var properties = await _unitOfWork.Properties.GetAllAsync(
                p => p.UserId == userId && !p.IsDeleted,
                includes: p => p.Images
            );

            if (properties == null || !properties.Any())
                return new List<HostedPropertyDTO>();

            var result = new List<HostedPropertyDTO>();

            foreach (var prop in properties)
            {
                var avgRating = await _reviewService.GetAverageRatingForPropertyAsync(prop.Id);
                var reviews = await _reviewService.GetReviewsForPropertyAsync(prop.Id);
                var reviewCount = reviews.Count();

                result.Add(new HostedPropertyDTO
                {
                    Id = prop.Id,
                    Title = prop.Title,
                    ImageUrl = prop.MainImageUrl,
                    Area = prop.Space,
                    Beds = prop.RoomCount,
                    Baths = prop.BathroomCount,
                    Price = prop.Price,
                    Rating = avgRating,
                    ReviewCount = reviewCount,
                    Location = $"{prop.City}, {prop.Country}"
                });
            }

            return result.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }

        public async Task<List<GetAllPropertiesDTO>> GetAllPropertiesAsync()
        {
            var properties = await _unitOfWork.Properties.GetAllAsync();
            if (properties == null || !properties.Any())
                return new List<GetAllPropertiesDTO>();

            return _mapper.Map<List<GetAllPropertiesDTO>>(properties);
        }


    }
}