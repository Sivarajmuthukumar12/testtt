/*
 * Folder: Interfaces
 * File: IRepository.cs
 * Purpose: Generic repository interface — defines standard CRUD operations for any entity.
 * Interview Tip: This is the Repository Pattern. It abstracts data access so services
 *                don't depend on EF Core directly. You can swap the database without
 *                changing any service code.
 */

namespace RetailOrderingSystem.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(int id);
    }
}
