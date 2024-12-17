using BusinessLogic.DTOs.Order;
using BusinessLogic.DTOs;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Utils.Implementation;

namespace StempedeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetAllOrdersNoFilter()
        {
            try
            {
                var response = await _orderService.GetAllOrdersNoFilterAsync();
                if (response.Success)
                {
                    return Ok(response);
                }
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving all orders");
                return StatusCode(500, ApiResponse<List<OrderDto>>.FailureResponse(
                    "An error occurred while retrieving orders.",
                    new List<string> { "Internal server error." }));
            }
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedList<OrderDto>>>> GetAllOrders([FromQuery] QueryParameters queryParameters)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid QueryParameters received.");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponse<PaginatedList<OrderDto>>.FailureResponse("Invalid pagination parameters.", errors));
            }

            try
            {
                var response = await _orderService.GetAllOrdersAsync(queryParameters);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving all orders.");
                return StatusCode(500, ApiResponse<PaginatedList<OrderDto>>.FailureResponse("An error occurred while retrieving orders.", new List<string> { "Internal server error." }));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                
                var response = await _orderService.GetOrderByIdAsync(id, "", "");

                if (!response.Success)
                {
                    return response.Message == "Order not found" ?
                        NotFound(response) : BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by ID: {Id}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{orderId}/deliveries/{deliveryId}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateDeliveryStatus(
            int orderId,
            int deliveryId,
            [FromBody] UpdateDeliveryStatusDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid UpdateDeliveryStatusDto received for OrderId: {OrderId}, DeliveryId: {DeliveryId}", orderId, deliveryId);
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(ApiResponse<string>.FailureResponse("Invalid input.", errors));
            }

            try
            {
                
                var response = await _orderService.UpdateDeliveryStatusAsync(orderId, deliveryId, updateDto, "Manager");
                if (response.Success)
                {
                    return NoContent();
                }
                else
                {
                    if (response.Message == "Delivery not found.")
                        return NotFound(response);
                    else if (response.Message == "Unauthorized access.")
                        return Forbid();
                    else
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating delivery status for OrderId: {OrderId}, DeliveryId: {DeliveryId}", orderId, deliveryId);
                return StatusCode(500, ApiResponse<string>.FailureResponse("An error occurred while updating delivery status.", new List<string> { "Internal server error." }));
            }
        }

    }
}

