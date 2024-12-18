using BusinessLogic.DTOs.SupportRequest;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace StempedeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportRequestsController : ControllerBase
    {
        private readonly ISupportRequestService _supportService;
        private readonly ILogger<SupportRequestsController> _logger;

        public SupportRequestsController(
            ISupportRequestService supportService,
            ILogger<SupportRequestsController> logger)
        {
            _supportService = supportService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<SupportRequestDto>>>> GetAllSupportRequests()
        {
            try
            {
                var response = await _supportService.GetAllSupportRequestsAsync();
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all support requests");
                return StatusCode(500, ApiResponse<List<SupportRequestDto>>.FailureResponse(
                    "An error occurred while retrieving all support requests.",
                    new List<string> { "Internal server error." }));
            }
        }
        [HttpGet("user/{username}")]
        public async Task<ActionResult<ApiResponse<List<SupportRequestDto>>>> GetAllByUser(string username)
        {
            try
            {
                var response = await _supportService.GetAllByUserAsync(username);
                if (!response.Success)
                {
                    return response.Message switch
                    {
                        "User not found" => NotFound(response),
                        _ => BadRequest(response)
                    };
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting support requests for user: {Username}", username);
                return StatusCode(500, ApiResponse<List<SupportRequestDto>>.FailureResponse(
                    "An error occurred while retrieving support requests.",
                    new List<string> { "Internal server error." }));
            }
        }

        [HttpPut("{supportId}/status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateSupportStatus(
            int supportId,
            [FromBody] UpdateSupportStatusDto updateDto,
            [FromQuery] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(
                    "Invalid input",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            try
            {
                var response = await _supportService.UpdateSupportStatusAsync(supportId, updateDto, username);
                if (!response.Success)
                {
                    return response.Message switch
                    {
                        "Support request not found" => NotFound(response),
                        "Unauthorized" => Forbid(),
                        _ => BadRequest(response)
                    };
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating support status for ID: {SupportId}", supportId);
                return StatusCode(500, ApiResponse<string>.FailureResponse(
                    "An error occurred while updating support status.",
                    new List<string> { "Internal server error." }));
            }
        }
    }
}
