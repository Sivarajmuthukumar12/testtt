/*
 * Folder: SeedData
 * File: DataSeeder.cs
 * Purpose: Seeds the database with initial data on application startup.
 *          Creates Admin user, Delivery Partner, Categories, and Products.
 * Who Calls It: Program.cs (on startup)
 * Interview Tip: Seeding ensures the app works immediately after first run.
 *                Always check if data exists before inserting to avoid duplicates.
 */

using RetailOrderingSystem.Constants;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.Helpers;
using RetailOrderingSystem.Models;

namespace RetailOrderingSystem.SeedData
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed Admin User
            if (!context.Users.Any(u => u.Email == "admin@retail.com"))
            {
                var admin = new User
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@retail.com",
                    PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                    Role = AppConstants.RoleAdmin,
                    PhoneNumber = "9999999999",
                    Address = "Admin Office"
                };
                context.Users.Add(admin);
            }

            // Seed Delivery Partner
            if (!context.Users.Any(u => u.Email == "delivery@retail.com"))
            {
                var delivery = new User
                {
                    FirstName = "Delivery",
                    LastName = "Partner",
                    Email = "delivery@retail.com",
                    PasswordHash = PasswordHelper.HashPassword("Delivery@123"),
                    Role = AppConstants.RoleDeliveryPartner,
                    PhoneNumber = "8888888888",
                    Address = "Delivery Hub"
                };
                context.Users.Add(delivery);
            }

            await context.SaveChangesAsync();

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Pizza", Description = "Freshly baked pizzas with various toppings" },
                    new() { Name = "Cold Drink", Description = "Refreshing cold beverages and sodas" },
                    new() { Name = "Bread", Description = "Freshly baked breads and garlic breads" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var pizzaCategoryId = context.Categories.First(c => c.Name == "Pizza").Id;
                var drinkCategoryId = context.Categories.First(c => c.Name == "Cold Drink").Id;
                var breadCategoryId = context.Categories.First(c => c.Name == "Bread").Id;

                var products = new List<Product>
                {
                    // Pizzas
                    new() { Name = "Margherita Pizza", Description = "Classic tomato sauce with mozzarella cheese",
                        Price = 199, StockQuantity = 50, MinimumStockLevel = 5, CategoryId = pizzaCategoryId },
                    new() { Name = "Farmhouse Pizza", Description = "Loaded with fresh vegetables and cheese",
                        Price = 249, StockQuantity = 40, MinimumStockLevel = 5, CategoryId = pizzaCategoryId },
                    new() { Name = "Veggie Pizza", Description = "Garden fresh vegetables on crispy base",
                        Price = 229, StockQuantity = 45, MinimumStockLevel = 5, CategoryId = pizzaCategoryId },

                    // Cold Drinks
                    new() { Name = "Pepsi", Description = "Chilled Pepsi 500ml",
                        Price = 40, StockQuantity = 100, MinimumStockLevel = 10, CategoryId = drinkCategoryId },
                    new() { Name = "Coca Cola", Description = "Chilled Coca Cola 500ml",
                        Price = 40, StockQuantity = 100, MinimumStockLevel = 10, CategoryId = drinkCategoryId },
                    new() { Name = "Sprite", Description = "Chilled Sprite 500ml",
                        Price = 40, StockQuantity = 100, MinimumStockLevel = 10, CategoryId = drinkCategoryId },

                    // Breads
                    new() { Name = "Garlic Bread", Description = "Crispy garlic bread with herb butter",
                        Price = 89, StockQuantity = 60, MinimumStockLevel = 8, CategoryId = breadCategoryId },
                    new() { Name = "Cheese Bread", Description = "Soft bread loaded with melted cheese",
                        Price = 99, StockQuantity = 55, MinimumStockLevel = 8, CategoryId = breadCategoryId }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
