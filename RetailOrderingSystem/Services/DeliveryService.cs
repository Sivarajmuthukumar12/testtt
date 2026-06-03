/*
 * Folder: Services
 * File: DeliveryService.cs
 * Purpose: Handles delivery partner operations — view assigned orders, accept, mark delivered.
 * Who Calls It: DeliveryController
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Constants;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Order;
using RetailOrderingSystem.Interfaces;

namespace RetailOrderingSystem.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public DeliveryService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<OrderDto>> GetAssignedOrdersAsync(int deliveryPartnerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .Include(o => o.DeliveryPartner)
                .Where(o => o.DeliveryPartnerId == deliveryPartnerId
                    && o.OrderStatus != AppConstants.StatusDelivered)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "",
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product!.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto> AcceptOrderAsync(int orderId, int deliveryPartnerId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.DeliveryPartnerId == deliveryPartnerId)
                ?? throw new UnauthorizedAccessException("Order not assigned to you.");

            if (order.OrderStatus != AppConstants.StatusPending)
                throw new InvalidOperationException("Only pending orders can be accepted.");

            order.OrderStatus = AppConstants.StatusAccepted;
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                FinalAmount = order.FinalAmount,
                OrderDate = order.OrderDate,
                CustomerName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : ""
            };
        }

        public async Task<OrderDto> MarkDeliveredAsync(int orderId, int deliveryPartnerId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.DeliveryPartnerId == deliveryPartnerId)
                ?? throw new UnauthorizedAccessException("Order not assigned to you.");

            order.OrderStatus = AppConstants.StatusDelivered;
            order.DeliveredDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

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

            return new OrderDto
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus,
                FinalAmount = order.FinalAmount,
                DeliveredDate = order.DeliveredDate,
                CustomerName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : ""
            };
        }
    }
}
