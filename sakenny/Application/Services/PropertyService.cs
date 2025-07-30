using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using sakenny.Application.DTO;
using sakenny.Application.Interfaces;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;

namespace sakenny.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PropertyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PropertyDTO> AddPropertyAsync(AddPropertyDTO model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model), "Property model cannot be null");
            try
            {
                var property = _mapper.Map<Property>(model);
                //add images
                // set images url to prop
                property = await _unitOfWork.Properties.AddAsync(property);
                //create permit
                //after this convert to the dto
                var propertyDto = _mapper.Map<PropertyDTO>(property);
                // to return it
                return propertyDto;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

    }
}