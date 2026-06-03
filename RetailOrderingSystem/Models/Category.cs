/*
 * Folder: Models
 * File: Category.cs
 * Purpose: Represents a product category (Pizza, Cold Drink, Bread).
 * Who Uses It: AppDbContext, CategoryService, ProductService
 * Interview Tip: One Category has Many Products — this is a One-To-Many relationship.
 */

namespace RetailOrderingSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Property: One Category → Many Products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
