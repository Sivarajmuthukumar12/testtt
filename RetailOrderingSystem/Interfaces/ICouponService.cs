using RetailOrderingSystem.DTOs.Coupon;

namespace RetailOrderingSystem.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllAsync();
        Task<CouponDto> CreateAsync(CreateCouponRequest request);
        Task DeleteAsync(int id);
        Task<ApplyCouponResponse> ApplyCouponAsync(int userId, string couponCode);
    }
}
