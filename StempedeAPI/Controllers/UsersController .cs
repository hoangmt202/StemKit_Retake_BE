using BusinessLogic.DTOs;
using BusinessLogic.DTOs.User;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StempedeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUnitOfWork unitOfWork, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("profile/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                var userProfile = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Status = user.Status
                };

                return Ok(new ApiResponse<UserProfileDto>
                {
                    Success = true,
                    Data = userProfile,
                    Message = "User profile retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for username: {Username}", username);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the profile."
                });
            }
        }

        [HttpPut("update-profile/{username}")]
        public async Task<IActionResult> UpdateUserProfile(string username, [FromBody] UserProfileUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid profile data."
                });
            }

            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                
                user.FullName = updateDto.FullName ?? user.FullName;
                user.Phone = updateDto.Phone ?? user.Phone;
                user.Address = updateDto.Address ?? user.Address;

          
                if (updateDto.Username != null && updateDto.Username != user.Username)
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Username cannot be changed."
                    });
                }

                _unitOfWork.GetRepository<User>().Update(user);
                await _unitOfWork.CompleteAsync();

                var updatedUserProfile = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Status = user.Status
                };

                return Ok(new ApiResponse<UserProfileDto>
                {
                    Success = true,
                    Data = updatedUserProfile,
                    Message = "User profile updated successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for username: {Username}", username);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating the profile."
                });
            }
        }
    }

}
