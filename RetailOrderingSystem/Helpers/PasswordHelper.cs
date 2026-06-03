/*
 * Folder: Helpers
 * File: PasswordHelper.cs
 * Purpose: Wraps BCrypt password hashing and verification.
 * Who Calls It: AuthService, DataSeeder
 * Interview Tip: BCrypt is a one-way hash — you can never reverse it.
 *                To verify, you hash the input and compare with stored hash.
 */

namespace RetailOrderingSystem.Helpers
{
    public static class PasswordHelper
    {
        // Hash a plain-text password using BCrypt (work factor 12)
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verify a plain-text password against a stored BCrypt hash
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
