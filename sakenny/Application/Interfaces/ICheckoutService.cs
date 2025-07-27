using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<PropertyCheckoutDTO> getPropertyByIdAsync(int id);
    }
}
