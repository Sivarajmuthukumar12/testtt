/*
 * Folder: Models
 * File: User.cs
 * Purpose: Represents the User entity in the database.
 *          This is the EF Core model that maps directly to the "Users" table in SQL Server.
 * Who Uses It: AppDbContext, AuthService, Repository<User>
 * Real-World Analogy: Like a membership card record in a store database.
 * Interview Tip: EF Core uses this class to generate the SQL table via Code First migrations.
 */

namespace RetailOrderingSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // BCrypt hashed password
        public string Role { get; set; } = "Customer";            // Admin, Customer, DeliveryPartner
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }                  // JWT refresh token
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties (EF Core uses these to build foreign key relationships)
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public LoyaltyPoint? LoyaltyPoint { get; set; }
    }
}
