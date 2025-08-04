using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _context;
        private readonly IImageService _imageService;

        public PropertyService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, ApplicationDBContext context)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
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

            // ? Apply service filtering in-memory
            if (filter.ServiceIds?.Any(id => id > 0) == true)
            {
                properties = properties
                    .Where(p => p.Services != null && p.Services.Any(s => filter.ServiceIds.Contains(s.Id)))
                    .ToList();
            }
            // Map result to DTO and return
            return _mapper.Map<List<PropertyDTO>>(properties);
        }
    }
}