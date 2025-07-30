using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IPropertyTypeService
    {
        Task<IEnumerable<GetAllTypeDTO>> GetAllAsync();
        Task AddTypeAsync(AddTypeDTO dto);
        Task UpdateTypeAsync(UpdateTypeDTO dto);
        Task DeleteTypeAsync(int id);
    }
}
