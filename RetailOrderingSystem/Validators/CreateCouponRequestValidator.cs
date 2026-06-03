using FluentValidation;
using RetailOrderingSystem.DTOs.Coupon;

namespace RetailOrderingSystem.Validators
{
    public class CreateCouponRequestValidator : AbstractValidator<CreateCouponRequest>
    {
        public CreateCouponRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required.")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

            RuleFor(x => x)
                .Must(x => x.DiscountPercentage > 0 || x.FixedDiscountAmount > 0)
                .WithMessage("Coupon must have either a percentage or fixed discount.");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(x => x.FixedDiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Fixed discount cannot be negative.");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");

            RuleFor(x => x.UsageLimit)
                .GreaterThan(0).WithMessage("Usage limit must be at least 1.");
        }
    }
}
