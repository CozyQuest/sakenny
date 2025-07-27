using AutoMapper;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;

namespace sakenny.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CheckoutService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PropertyCheckoutDTO> getPropertyByIdAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with ID {id} was not found.");
            }
            var propertyCheckoutDTO = _mapper.Map<PropertyCheckoutDTO>(property);
            return propertyCheckoutDTO;
        }
    }
}
