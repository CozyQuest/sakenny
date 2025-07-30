using sakenny.Application.DTO;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public interface IPropertyServicesService
    {
        Task<IEnumerable<GetAllServiceDTO>> GetAllAsync();
        Task AddServiceAsync(AddServiceDTO dto);
        Task UpdateServiceAsync(UpdateServiceDTO dto);
        Task DeleteServiceAsync(int id);
    }
}
