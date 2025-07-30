using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PropertyTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetAllTypeDTO>> GetAllAsync()
        {
            var types = await _unitOfWork.PropertyTypes.GetAllAsync();
            return _mapper.Map<IEnumerable<GetAllTypeDTO>>(types);
        }

        public async Task AddTypeAsync(AddTypeDTO dto)
        {
            var entity = _mapper.Map<PropertyType>(dto);
            await _unitOfWork.PropertyTypes.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTypeAsync(int id)
        {
            var existing = await _unitOfWork.PropertyTypes.GetByIdAsync(id);
            if (existing == null || existing.IsDeleted)
                throw new KeyNotFoundException("Property type not found");

            _unitOfWork.PropertyTypes.SoftDeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateTypeAsync(UpdateTypeDTO dto)
        {
            var existing = await _unitOfWork.PropertyTypes.GetByIdAsync(dto.Id);
            if (existing == null || existing.IsDeleted)
                throw new KeyNotFoundException("Property type not found");

            _mapper.Map(dto, existing);
            _unitOfWork.PropertyTypes.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }

}
