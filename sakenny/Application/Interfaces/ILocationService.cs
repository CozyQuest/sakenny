using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface ILocationService
    {
        Task<List<GetLocationDTO>> GetLocationsAsync();

    }
}
