using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sakenny.Application.DTO;
using sakenny.Application.DTO.sakenny.DAL.DTOs;

namespace sakenny.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDTO> AddPropertyAsync(AddPropertyDTO model, string Id);
        Task<List<PropertyDTO>> GetFilteredPropertiesAsync(PropertyFilterDTO filterDto);
        Task<PropertyDTO> UpdatePropertyAsync(int id, UpdatePropertyDTO model, string userId);
        Task<PropertyDTO> GetPropertyDetailsAsync(int id);
        Task<IEnumerable<OwnedPropertyDTO>> GetUserOwnedPropertiesAsync(string userId);
        Task<List<OwnedPropertyDTO>> GetTopRatedPropertiesAsync();
        Task<PagedResultDTO<GetAllPropertiesDTO>> GetAllPropertiesAsync(PaginationDTO pagination);

    }
}