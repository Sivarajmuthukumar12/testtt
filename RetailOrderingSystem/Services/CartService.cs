/*
 * Folder: Services
 * File: CartService.cs
 * Purpose: Manages shopping cart operations — add, update, remove, clear items.
 * Who Calls It: CartController
 * Flow: CartController → CartService → AppDbContext → SQL Server
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.DTOs.Cart;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddItemAsync(int userId, AddToCartRequest request)
        {
            // Validate product exists and is active
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive)
                ?? throw new KeyNotFoundException("Product not found or inactive.");

            var cart = await GetOrCreateCartAsync(userId);

            // Check if item already in cart — update quantity instead of adding duplicate
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            await _context.SaveChangesAsync();

            // Reload cart with product details
            return await GetCartAsync(userId);
        }

        public async Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
                ?? throw new KeyNotFoundException("Cart item not found.");

            if (request.Quantity <= 0)
            {
                // Remove item if quantity is 0 or less
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }

        public async Task<CartDto> RemoveItemAsync(int userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
                ?? throw new KeyNotFoundException("Cart item not found.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
        }

        // Gets existing cart or creates a new one for the user
        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private static CartDto MapToDto(Cart cart) => new()
        {
            CartId = cart.Id,
            Items = cart.CartItems.Select(ci => new CartItemDto
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
}
