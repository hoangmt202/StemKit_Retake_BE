using AutoMapper;
using BusinessLogic.DTOs.SupportRequest;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Implementation
{
    public class SupportRequestService : ISupportRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SupportRequestService> _logger;

        public SupportRequestService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SupportRequestService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<List<SupportRequestDto>>> GetAllByUserAsync(string username)
        {
            try
            {
                _logger.LogInformation("Getting support requests for user: {Username}", username);

                var user = await _unitOfWork.GetRepository<User>()
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return ApiResponse<List<SupportRequestDto>>.FailureResponse(
                        "User not found",
                        new List<string> { "The specified user does not exist." });
                }

                var supportRequests = await _unitOfWork.GetRepository<SupportRequest>()
                    .GetAllQueryable(includeProperties: "User,Order")
                    .Where(s => s.User.Username == username)
                    .OrderByDescending(s => s.SupportId)
                    .ToListAsync();

                var supportRequestDtos = _mapper.Map<List<SupportRequestDto>>(supportRequests);
                return ApiResponse<List<SupportRequestDto>>.SuccessResponse(
                    supportRequestDtos,
                    "Support requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting support requests for user: {Username}", username);
                return ApiResponse<List<SupportRequestDto>>.FailureResponse(
                    "Failed to retrieve support requests",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateSupportStatusAsync(
            int supportId,
            UpdateSupportStatusDto updateDto,
            string username)
        {
            try
            {
                var supportRequest = await _unitOfWork.GetRepository<SupportRequest>()
                    .GetAllQueryable(includeProperties: "User")
                    .FirstOrDefaultAsync(s => s.SupportId == supportId);

                if (supportRequest == null)
                {
                    return ApiResponse<string>.FailureResponse(
                        "Support request not found",
                        new List<string> { "The specified support request does not exist." });
                }

                if (supportRequest.User.Username != username)
                {
                    return ApiResponse<string>.FailureResponse(
                        "Unauthorized",
                        new List<string> { "You don't have permission to update this support request." });
                }

                supportRequest.SupportStatus = updateDto.SupportStatus;
                await _unitOfWork.CompleteAsync();

                return ApiResponse<string>.SuccessResponse(
                    "Support status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating support status for ID: {SupportId}", supportId);
                return ApiResponse<string>.FailureResponse(
                    "Failed to update support status",
                    new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<List<SupportRequestDto>>> GetAllSupportRequestsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all support requests");

                var supportRequests = await _unitOfWork.GetRepository<SupportRequest>()
                    .GetAllQueryable(includeProperties: "User,Order")
                    .OrderByDescending(s => s.SupportId)
                    .ToListAsync();

                if (!supportRequests.Any())
                {
                    return ApiResponse<List<SupportRequestDto>>.SuccessResponse(
                        new List<SupportRequestDto>(),
                        "No support requests found.");
                }

                var supportRequestDtos = _mapper.Map<List<SupportRequestDto>>(supportRequests);
                return ApiResponse<List<SupportRequestDto>>.SuccessResponse(
                    supportRequestDtos,
                    "All support requests retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all support requests");
                return ApiResponse<List<SupportRequestDto>>.FailureResponse(
                    "Failed to retrieve support requests",
                    new List<string> { ex.Message });
            }
        }
    }
}
