/*
 * Folder: Services
 * File: CouponService.cs
 * Purpose: Manages coupon creation, deletion, and validation/application.
 * Who Calls It: CouponController
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Coupon;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _context;

        public CouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync()
        {
            return await _context.Coupons
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        public async Task<CouponDto> CreateAsync(CreateCouponRequest request)
        {
            // Validate at least one discount type is set
            if (request.DiscountPercentage <= 0 && request.FixedDiscountAmount <= 0)
                throw new InvalidOperationException("Coupon must have either a percentage or fixed discount.");

            var coupon = new Coupon
            {
                Code = request.Code.ToUpper(),
                DiscountPercentage = request.DiscountPercentage,
                FixedDiscountAmount = request.FixedDiscountAmount,
                MinimumOrderAmount = request.MinimumOrderAmount,
                ExpiryDate = request.ExpiryDate,
                UsageLimit = request.UsageLimit
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return MapToDto(coupon);
        }

        public async Task DeleteAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id)
                ?? throw new KeyNotFoundException($"Coupon with ID {id} not found.");

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplyCouponResponse> ApplyCouponAsync(int userId, string couponCode)
        {
            // Get user's cart total
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var cartTotal = cart.CartItems.Sum(ci => ci.Product!.Price * ci.Quantity);

            // Validate coupon
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == couponCode.ToUpper() && c.IsActive);

            if (coupon == null)
                throw new InvalidOperationException("Coupon code not found.");

            if (coupon.ExpiryDate < DateTime.UtcNow)
                throw new InvalidOperationException("Coupon has expired.");

            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new InvalidOperationException("Coupon usage limit reached.");

            if (cartTotal < coupon.MinimumOrderAmount)
                throw new InvalidOperationException(
                    $"Minimum order amount of ₹{coupon.MinimumOrderAmount} required.");

            // Calculate discount
            decimal discountAmount = 0;
            if (coupon.DiscountPercentage > 0)
                discountAmount = cartTotal * (coupon.DiscountPercentage / 100);
            else
                discountAmount = coupon.FixedDiscountAmount;

            // Discount cannot exceed cart total
            discountAmount = Math.Min(discountAmount, cartTotal);

            return new ApplyCouponResponse
            {
                CouponCode = coupon.Code,
                OriginalAmount = cartTotal,
                DiscountAmount = discountAmount,
                FinalAmount = cartTotal - discountAmount,
                Message = $"Coupon applied! You save ₹{discountAmount:F2}"
            };
        }

        private static CouponDto MapToDto(Coupon c) => new()
        {
            Id = c.Id,
            Code = c.Code,
            DiscountPercentage = c.DiscountPercentage,
            FixedDiscountAmount = c.FixedDiscountAmount,
            MinimumOrderAmount = c.MinimumOrderAmount,
            ExpiryDate = c.ExpiryDate,
            UsageLimit = c.UsageLimit,
            UsedCount = c.UsedCount,
            IsActive = c.IsActive
        };
    }
}
