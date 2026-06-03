using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.DTOs.Order;
using RetailOrderingSystem.Interfaces;
using System.Security.Claims;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetRole() =>
            User.FindFirstValue(ClaimTypes.Role)!;

        /// <summary>Place a new order from cart (Customer only)</summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var result = await _orderService.PlaceOrderAsync(GetUserId(), request);
            return StatusCode(201, result);
        }

        /// <summary>Get current customer's order history</summary>
        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var result = await _orderService.GetMyOrdersAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>Get order by ID (Customer sees own, Admin sees all)</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id, GetUserId(), GetRole());
            return Ok(result);
        }

        /// <summary>Track order status</summary>
        [HttpGet("{id}/track")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> TrackOrder(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(id, GetUserId(), GetRole());
            return Ok(new { result.Id, result.OrderStatus, result.OrderDate, result.DeliveredDate });
        }

        /// <summary>Reorder from a previous order (Customer only)</summary>
        [HttpPost("{id}/reorder")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Reorder(int id)
        {
            var result = await _orderService.ReorderAsync(id, GetUserId());
            return Ok(result);
        }

        /// <summary>Get all orders with pagination (Admin only)</summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrdersAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>Update order status (Admin only)</summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateStatusAsync(id, request.Status);
            return Ok(result);
        }

        /// <summary>Assign delivery partner to order (Admin only)</summary>
        [HttpPut("{id}/assign-delivery")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDelivery(int id, [FromBody] AssignDeliveryRequest request)
        {
            await _orderService.AssignDeliveryPartnerAsync(id, request.DeliveryPartnerId);
            return Ok(new { Message = "Delivery partner assigned successfully." });
        }
    }
}
