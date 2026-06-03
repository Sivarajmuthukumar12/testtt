using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderingSystem.DTOs.Coupon;
using RetailOrderingSystem.Interfaces;
using System.Security.Claims;

namespace RetailOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>Get all coupons (Admin only)</summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _couponService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Create a new coupon (Admin only)</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCouponRequest request)
        {
            var result = await _couponService.CreateAsync(request);
            return StatusCode(201, result);
        }

        /// <summary>Delete a coupon (Admin only)</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _couponService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>Apply a coupon to cart (Customer only)</summary>
        [HttpPost("apply")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Apply([FromBody] ApplyCouponRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _couponService.ApplyCouponAsync(userId, request.CouponCode);
            return Ok(result);
        }
    }
}
