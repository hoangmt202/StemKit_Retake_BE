using AutoMapper;
using DataAccess.Data;
using DataAccess.Entities;
using BusinessLogic.DTOs.Subcategory;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.DTOs.Product;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services.Implementation
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubcategoryService> _logger;

        public SubcategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SubcategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
         
        }

        /// <summary>
        /// Retrieves all subcategories from the database.
        /// </summary>
        /// <returns>List of subcategories.</returns>
        public async Task<IEnumerable<ReadSubcategoryDto>> GetAllSubcategoriesAsync()
        {
            var subcategories = await _unitOfWork.GetRepository<Subcategory>().GetAllAsync();
            return _mapper.Map<IEnumerable<ReadSubcategoryDto>>(subcategories);
        }

        public async Task<ReadSubcategoryDto?> GetCategoryByIdAsync(int categoryId)
        {
            var subcategory = await _unitOfWork.GetRepository<Subcategory>().GetByIdAsync(categoryId);
            return subcategory != null ? _mapper.Map<ReadSubcategoryDto>(subcategory) : null;
        }

        public async Task<ReadSubcategoryDto> CreateSubcategoryAsync(CreateSubcategoryDto createDto)
        {
            var subcategory = _mapper.Map<Subcategory>(createDto);
            await _unitOfWork.GetRepository<Subcategory>().AddAsync(subcategory);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Subcategory created with ID {SubcategoryId}.", subcategory.SubcategoryId);
            var createdSubcategory = await _unitOfWork.GetRepository<Subcategory>().GetAsync(
                s => s.SubcategoryId == subcategory.SubcategoryId,
                includeProperties: "Products");
            return _mapper.Map<ReadSubcategoryDto>(createdSubcategory);
        }

        public async Task<bool> UpdateSubcategoryAsync(int id, CreateSubcategoryDto updateDto)
        {
            var subcategory = await _unitOfWork.GetRepository<Subcategory>().GetByIdAsync(id);
            if (subcategory == null)
            {
                return false;
            }
            _mapper.Map(updateDto, subcategory);
            _unitOfWork.GetRepository<Subcategory>().Update(subcategory);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Subcategory with ID {SubcategoryId} updated.", subcategory.SubcategoryId);
            return true;
        }

        public async Task<bool> DeleteSubcategoryAsync(int id)
        {
            var subcategory = await _unitOfWork.GetRepository<Subcategory>().GetByIdAsync(id);
            if (subcategory == null)
            {
                return false;
            }
            _unitOfWork.GetRepository<Subcategory>().Delete(subcategory);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Subcategory with ID {SubcategoryId} deleted.", subcategory.SubcategoryId);
            return true;
        }
    }
}
