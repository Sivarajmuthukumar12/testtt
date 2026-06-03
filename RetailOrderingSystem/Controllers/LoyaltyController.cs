using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.Interfaces;
using System.Security.Claims;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/loyalty")]
    [Authorize(Roles = "Customer")]
    public class LoyaltyController : ControllerBase
    {
        private readonly ILoyaltyService _loyaltyService;

        public LoyaltyController(ILoyaltyService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        /// <summary>Get current customer's loyalty points balance</summary>
        [HttpGet]
        public async Task<IActionResult> GetPoints()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _loyaltyService.GetPointsAsync(userId);
            return Ok(result);
        }
    }
}
