/*
 * Folder: Services
 * File: OrderService.cs
 * Purpose: Core business logic for placing and managing orders.
 * Flow: Validate cart → Apply coupon → Redeem loyalty → Create order →
 *       Reduce inventory → Award loyalty points → Clear cart → Send email
 * Who Calls It: OrderController
 * Interview Tip: This service orchestrates multiple repositories in one transaction.
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Constants;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Cart;
using RetailOrderingSystem.DTOs.Order;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(AppDbContext context, IEmailService emailService,
            ILogger<OrderService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<OrderDto> PlaceOrderAsync(int customerId, PlaceOrderRequest request)
        {
            // Step 1: Load customer's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty. Add items before placing an order.");

            // Step 2: Validate stock for all items
            foreach (var item in cart.CartItems)
            {
                if (item.Product!.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{item.Product.Name}'. Available: {item.Product.StockQuantity}");
            }

            // Step 3: Calculate total
            decimal totalAmount = cart.CartItems.Sum(ci => ci.Product!.Price * ci.Quantity);
            decimal discountAmount = 0;

            // Step 4: Apply coupon if provided
            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == request.CouponCode.ToUpper() && c.IsActive);

                if (coupon == null) throw new InvalidOperationException("Invalid coupon code.");
                if (coupon.ExpiryDate < DateTime.UtcNow) throw new InvalidOperationException("Coupon expired.");
                if (coupon.UsedCount >= coupon.UsageLimit) throw new InvalidOperationException("Coupon limit reached.");
                if (totalAmount < coupon.MinimumOrderAmount)
                    throw new InvalidOperationException($"Minimum order ₹{coupon.MinimumOrderAmount} required.");

                discountAmount = coupon.DiscountPercentage > 0
                    ? totalAmount * (coupon.DiscountPercentage / 100)
                    : coupon.FixedDiscountAmount;

                discountAmount = Math.Min(discountAmount, totalAmount);
                coupon.UsedCount++;
            }

            // Step 5: Apply loyalty points redemption
            int loyaltyPointsUsed = 0;
            if (request.LoyaltyPointsToRedeem > 0)
            {
                var loyalty = await _context.LoyaltyPoints
                    .FirstOrDefaultAsync(lp => lp.UserId == customerId);

                if (loyalty == null || loyalty.Points < request.LoyaltyPointsToRedeem)
                    throw new InvalidOperationException("Insufficient loyalty points.");

                // 100 points = ₹10 discount
                decimal loyaltyDiscount = (request.LoyaltyPointsToRedeem / 100m) *
                    AppConstants.LoyaltyRupeesPerHundredPoints;

                discountAmount += loyaltyDiscount;
                loyaltyPointsUsed = request.LoyaltyPointsToRedeem;
                loyalty.Points -= loyaltyPointsUsed;
            }

            decimal finalAmount = Math.Max(0, totalAmount - discountAmount);

            // Step 6: Calculate loyalty points to earn (10 pts per ₹100)
            int pointsEarned = (int)(finalAmount / 100) * AppConstants.LoyaltyPointsPerHundredRupees;

            // Step 7: Create the order
            var order = new Order
            {
                UserId = customerId,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                OrderStatus = AppConstants.StatusPending,
                CouponCode = request.CouponCode?.ToUpper(),
                LoyaltyPointsUsed = loyaltyPointsUsed,
                LoyaltyPointsEarned = pointsEarned,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product!.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Step 8: Reduce inventory and record transactions
            foreach (var item in cart.CartItems)
            {
                var oldQty = item.Product!.StockQuantity;
                item.Product.StockQuantity -= item.Quantity;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = item.ProductId,
                    OldQuantity = oldQty,
                    NewQuantity = item.Product.StockQuantity,
                    TransactionType = AppConstants.TransactionOrderDeduction,
                    Notes = $"Order placed"
                });
            }

            // Step 9: Award loyalty points
            var loyaltyRecord = await _context.LoyaltyPoints
                .FirstOrDefaultAsync(lp => lp.UserId == customerId);
            if (loyaltyRecord != null)
                loyaltyRecord.Points += pointsEarned;

            // Step 10: Clear the cart
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Step 11: Send confirmation email (fire and forget)
            var customer = await _context.Users.FindAsync(customerId);
            if (customer != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendOrderConfirmationEmailAsync(
                            customer.Email, customer.FirstName, order.Id, finalAmount);
                    }
                    catch (Exception ex) { _logger.LogWarning("Order email failed: {msg}", ex.Message); }
                });
            }

            _logger.LogInformation("Order #{orderId} placed by user {userId}", order.Id, customerId);
            return await GetOrderDtoAsync(order.Id);
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.DeliveryPartner)
                .Where(o => o.UserId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, int requestingUserId, string role)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.DeliveryPartner)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new KeyNotFoundException($"Order #{orderId} not found.");

            // Customers can only see their own orders
            if (role == AppConstants.RoleCustomer && order.UserId != requestingUserId)
                throw new UnauthorizedAccessException("You can only view your own orders.");

            return MapToDto(order);
        }

        public async Task<OrderDto> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId)
                ?? throw new KeyNotFoundException($"Order #{orderId} not found.");

            order.OrderStatus = status;

            if (status == AppConstants.StatusDelivered)
            {
                order.DeliveredDate = DateTime.UtcNow;

                // Send delivered email
                if (order.User != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendOrderDeliveredEmailAsync(
                                order.User.Email, order.User.FirstName, order.Id);
                        }
                        catch { }
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await GetOrderDtoAsync(orderId);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int page, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.DeliveryPartner)
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => MapToDto(o))
                .ToListAsync();
        }

        public async Task<CartDto> ReorderAsync(int orderId, int customerId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId)
                ?? throw new KeyNotFoundException("Order not found.");

            // Get or create cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null)
            {
                cart = new Cart { UserId = customerId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Add all items from previous order to cart
            foreach (var item in order.OrderItems)
            {
                if (item.Product?.IsActive == true)
                {
                    var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                    if (existing != null)
                        existing.Quantity += item.Quantity;
                    else
                        cart.CartItems.Add(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                }
            }

            await _context.SaveChangesAsync();

            // Return updated cart
            var updatedCart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstAsync(c => c.UserId == customerId);

            return new CartDto
            {
                CartId = updatedCart.Id,
                Items = updatedCart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? string.Empty,
                    UnitPrice = ci.Product?.Price ?? 0,
                    Quantity = ci.Quantity,
                    ImageUrl = ci.Product?.ImageUrl
                }).ToList()
            };
        }

        public async Task AssignDeliveryPartnerAsync(int orderId, int deliveryPartnerId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new KeyNotFoundException($"Order #{orderId} not found.");

            var partner = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == deliveryPartnerId
                    && u.Role == AppConstants.RoleDeliveryPartner)
                ?? throw new KeyNotFoundException("Delivery partner not found.");

            order.DeliveryPartnerId = deliveryPartnerId;
            await _context.SaveChangesAsync();
        }

        private async Task<OrderDto> GetOrderDtoAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.DeliveryPartner)
                .FirstAsync(o => o.Id == orderId);

            return MapToDto(order);
        }

        private static OrderDto MapToDto(Order o) => new()
        {
            Id = o.Id,
            TotalAmount = o.TotalAmount,
            DiscountAmount = o.DiscountAmount,
            FinalAmount = o.FinalAmount,
            OrderStatus = o.OrderStatus,
            CouponCode = o.CouponCode,
            LoyaltyPointsUsed = o.LoyaltyPointsUsed,
            LoyaltyPointsEarned = o.LoyaltyPointsEarned,
            OrderDate = o.OrderDate,
            DeliveredDate = o.DeliveredDate,
            UserId = o.UserId,
            CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : string.Empty,
            DeliveryPartnerName = o.DeliveryPartner != null
                ? $"{o.DeliveryPartner.FirstName} {o.DeliveryPartner.LastName}" : null,
            Items = o.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}
