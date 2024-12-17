using BusinessLogic.DTOs.Product;
using BusinessLogic.DTOs.Subcategory;

namespace BusinessLogic.Services.Interfaces
{
    public interface ISubcategoryService
    {
        Task<IEnumerable<ReadSubcategoryDto>> GetAllSubcategoriesAsync();
        Task<ReadSubcategoryDto?> GetCategoryByIdAsync(int categoryId);
        Task<ReadSubcategoryDto> CreateSubcategoryAsync(CreateSubcategoryDto createDto);
        Task<bool> UpdateSubcategoryAsync(int id, CreateSubcategoryDto updateDto);
        Task<bool> DeleteSubcategoryAsync(int id);

    }
}
