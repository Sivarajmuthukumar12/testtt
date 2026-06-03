using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.Interfaces;
using System.Security.Claims;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    [Authorize(Roles = "DeliveryPartner")]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get all orders assigned to this delivery partner</summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetAssignedOrders()
        {
            var result = await _deliveryService.GetAssignedOrdersAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>Accept an assigned order</summary>
        [HttpPut("orders/{id}/accept")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var result = await _deliveryService.AcceptOrderAsync(id, GetUserId());
            return Ok(result);
        }

        /// <summary>Mark an order as delivered</summary>
        [HttpPut("orders/{id}/deliver")]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var result = await _deliveryService.MarkDeliveredAsync(id, GetUserId());
            return Ok(result);
        }
    }
}
