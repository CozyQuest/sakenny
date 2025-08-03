using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sakenny.Application.DTO;

namespace sakenny.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDTO> AddPropertyAsync(AddPropertyDTO model, string Id);
        Task<List<PropertyDTO>> GetFilteredPropertiesAsync(PropertyFilterDTO filterDto);
        Task<PropertyDTO> UpdatePropertyAsync(int id, UpdatePropertyDTO model, string userId);
        Task<PropertyDTO> GetPropertyDetailsAsync(int id);

    }
}