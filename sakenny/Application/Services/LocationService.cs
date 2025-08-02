using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL;
using Microsoft.EntityFrameworkCore;

namespace sakenny.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDBContext _context;

        public LocationService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<GetLocationDTO>> GetLocationsAsync()
        {
            var properties = await _context.Properties
                .Where(p => !p.IsDeleted)
                .Select(p => new
                {
                    p.Country,
                    p.City,
                    p.District
                })
                .ToListAsync();

            var groupedByCountry = properties
                .GroupBy(p => p.Country)
                .Select(countryGroup => new GetLocationDTO
                {
                    Country = countryGroup.Key,
                    Cities = countryGroup
                        .GroupBy(p => p.City)
                        .Select(cityGroup => new GetCityDTO
                        {
                            City = cityGroup.Key,
                            Districts = cityGroup
                                .Select(p => p.District)
                                .Distinct()
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return groupedByCountry;
        }
    }
 }
