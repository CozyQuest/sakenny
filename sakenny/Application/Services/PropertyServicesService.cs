using AutoMapper;
using sakenny.Application.DTO;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class PropertyServicesService : IPropertyServicesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PropertyServicesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _unitOfWork.Services.GetAllAsync();
        }

        public async Task AddServiceAsync(AddServiceDTO dto)
        {
            var service = _mapper.Map<Service>(dto);
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateServiceAsync(UpdateServiceDTO dto)
        {
            var existing = await _unitOfWork.Services.GetByIdAsync(dto.Id);
            if (existing == null) throw new Exception("Service not found");

            _mapper.Map(dto, existing); // Map into existing
            _unitOfWork.Services.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(long id)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(id);
            if (service == null) throw new Exception("Service not found");

            _unitOfWork.Services.SoftDeleteAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
