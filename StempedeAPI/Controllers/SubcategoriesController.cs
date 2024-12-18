using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.DTOs.Subcategory;
using BusinessLogic.DTOs;
using BusinessLogic.DTOs.Product;
using BusinessLogic.Services.Implementation;

namespace StempedeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly ILogger<SubcategoriesController> _logger;

        public SubcategoriesController(ISubcategoryService subcategoryService, ILogger<SubcategoriesController> logger)
        {
            _subcategoryService = subcategoryService;
            _logger = logger;
        }

    
        [HttpGet("get-all")]
        [AllowAnonymous] 
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadSubcategoryDto>>>> GetAllSubcategories()
        {
            try
            {
                var subcategories = await _subcategoryService.GetAllSubcategoriesAsync();
                return Ok(new ApiResponse<IEnumerable<ReadSubcategoryDto>>
                {
                    Success = true,
                    Data = subcategories,
                    Message = "Subcategories retrieved successfully.",
                    Errors = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving subcategories.");
                return StatusCode(500, new ApiResponse<IEnumerable<ReadSubcategoryDto>>
                {
                    Success = false,
                    Data = null,
                    Message = "An error occurred while retrieving subcategories.",
                    Errors = new List<string> { "Internal server error." }
                });
            }
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ReadSubcategoryDto>>> GetSubcategoryById(int id)
        {
            var subcategory = await _subcategoryService.GetCategoryByIdAsync(id);
            if (subcategory == null)
            {
                return NotFound(new ApiResponse<ReadSubcategoryDto>
                {
                    Success = false,
                    Message = $"Subcategory with ID {id} not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<ReadSubcategoryDto>
            {
                Success = true,
                Data = subcategory,
                Message = "Subcategory retrieved successfully."
            });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ReadSubcategoryDto>>> CreateSubcategory([FromBody] CreateSubcategoryDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<ReadSubcategoryDto>
                {
                    Success = false,
                    Message = "Invalid subcategory data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var createdSubcategory = await _subcategoryService.CreateSubcategoryAsync(createDto);
                return CreatedAtAction(nameof(GetSubcategoryById), new { id = createdSubcategory.SubcategoryId }, new ApiResponse<ReadSubcategoryDto>
                {
                    Success = true,
                    Data = createdSubcategory,
                    Message = "Subcategory created successfully."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating subcategory: {ex.Message}");
                return StatusCode(500, new ApiResponse<ReadSubcategoryDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the subcategory.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> UpdateSubcategory(int id, [FromBody] CreateSubcategoryDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid subcategory data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                var success = await _subcategoryService.UpdateSubcategoryAsync(id, updateDto);
                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Subcategory not found."
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = null,
                    Message = "Subcategory updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subcategory.");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating the subcategory.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }   
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<string>>> DeleteSubcategory(int id)
        {
            try
            {
                var success = await _subcategoryService.DeleteSubcategoryAsync(id);
                if (!success)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Subcategory not found."
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = null,
                    Message = "Subcategory deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subcategory.");
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while deleting the subcategory.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
