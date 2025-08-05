using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IRentedPropertyService
    {
        Task<IEnumerable<RentedPropertyDTO>> GetRentedPropertiesByUserAsync(string userId);
        Task<HostEarningsDTO> GetHostEarningsAsync(string hostId);
    }
}
