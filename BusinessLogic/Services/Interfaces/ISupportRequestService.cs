using BusinessLogic.DTOs.SupportRequest;
using BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface ISupportRequestService
    {
        Task<ApiResponse<List<SupportRequestDto>>> GetAllByUserAsync(string username);
        Task<ApiResponse<string>> UpdateSupportStatusAsync(int supportId, UpdateSupportStatusDto updateDto, string username);
        Task<ApiResponse<List<SupportRequestDto>>> GetAllSupportRequestsAsync();
    }
}
