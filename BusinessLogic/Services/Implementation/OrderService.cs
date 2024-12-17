using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.DTOs.Order;
using BusinessLogic.DTOs.Reporting;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Utils.Implementation;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BusinessLogic.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ApiResponse<List<OrderDto>>> GetAllOrdersNoFilterAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all orders without pagination");

                var orders = await _unitOfWork.GetRepository<Order>()
                    .GetAllQueryable(includeProperties: "User,OrderDetails.Product")
                    .OrderBy(o => o.OrderId)
                    .ToListAsync();

                var orderDtos = _mapper.Map<List<OrderDto>>(orders);

                return ApiResponse<List<OrderDto>>.SuccessResponse(orderDtos, "Orders retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders");
                return ApiResponse<List<OrderDto>>.FailureResponse("Failed to retrieve orders.",
                    new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<PaginatedList<OrderDto>>> GetAllOrdersAsync(QueryParameters queryParameters)
        {
            try
            {
                _logger.LogInformation("Fetching all orders for reporting with pagination. PageNumber: {PageNumber}, PageSize: {PageSize}",
                    queryParameters.PageNumber, queryParameters.PageSize);

                var orderQuery = _unitOfWork.GetRepository<Order>()
                                            .GetAllQueryable(includeProperties: "User,OrderDetails.Product")  
                                            .OrderBy(o => o.OrderId);

                var paginatedOrders = await PaginatedList<Order>.CreateAsync(orderQuery, queryParameters.PageNumber, queryParameters.PageSize);

                var orderDtos = _mapper.Map<List<OrderDto>>(paginatedOrders.Items);

                var paginatedOrderDtos = new PaginatedList<OrderDto>(orderDtos, paginatedOrders.TotalCount, paginatedOrders.PageIndex, queryParameters.PageSize);

                return ApiResponse<PaginatedList<OrderDto>>.SuccessResponse(paginatedOrderDtos, "Orders retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders.");
                return ApiResponse<PaginatedList<OrderDto>>.FailureResponse("Failed to retrieve orders.", new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId, string currentUsername, string userRole)
        {
            try
            {
                _logger.LogInformation("Fetching order with ID: {OrderId}", orderId);

                var order = await _unitOfWork.GetRepository<Order>()
                    .GetAllQueryable(includeProperties: "User,OrderDetails.Product")
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found.", orderId);
                    return ApiResponse<OrderDto>.FailureResponse("Order not found.",
                        new List<string> { "The specified order does not exist." });
                }

                var orderDto = _mapper.Map<OrderDto>(order);
                return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order with ID: {OrderId}", orderId);
                return ApiResponse<OrderDto>.FailureResponse("Failed to retrieve the order.",
                    new List<string> { ex.Message });
            }
        }

        public ApiResponse<List<string>> GetAvailableDeliveryStatuses(int orderId)
        {
            try
            {
                var order = _unitOfWork.GetRepository<Order>()
                    .GetAllQueryable()
                    .FirstOrDefault(o => o.OrderId == orderId);

                if (order == null)
                {
                    return ApiResponse<List<string>>.FailureResponse("Order not found",
                        new List<string> { "The specified order does not exist." });
                }

                var statuses = new List<string>();
                switch (order.DeliveryStatus)
                {
                    case "Đã đặt hàng":
                        statuses.Add("Đang giao hàng");
                        break;
                    case "Đang giao hàng":
                        statuses.Add("Đã giao hàng");
                        break;
                }
                statuses.Add(order.DeliveryStatus); // Thêm trạng thái hiện tại

                return ApiResponse<List<string>>.SuccessResponse(statuses,
                    "Available statuses retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available statuses for OrderId: {OrderId}", orderId);
                return ApiResponse<List<string>>.FailureResponse("Failed to get available statuses",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> UpdateDeliveryStatusAsync(int orderId, UpdateDeliveryStatusDto updateDto)
        {
            try
            {
                var order = await _unitOfWork.GetRepository<Order>()
                    .GetAllQueryable()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return ApiResponse<string>.FailureResponse("Order not found",
                        new List<string> { "The specified order does not exist." });
                }

                if (!IsValidStatusTransition(order.DeliveryStatus, updateDto.DeliveryStatus))
                {
                    return ApiResponse<string>.FailureResponse("Invalid status transition",
                        new List<string> { "Không thể chuyển sang trạng thái này." });
                }

                order.DeliveryStatus = updateDto.DeliveryStatus;
                await _unitOfWork.CompleteAsync();

                return ApiResponse<string>.SuccessResponse("Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for OrderId: {OrderId}", orderId);
                return ApiResponse<string>.FailureResponse("Failed to update status",
                    new List<string> { ex.Message });
            }
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                ("Đã đặt hàng", "Đang giao hàng") => true,
                ("Đang giao hàng", "Đã giao hàng") => true,
                _ => false
            };
        }

    }
}