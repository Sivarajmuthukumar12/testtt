/*
 * Folder: Repositories
 * File: Repository.cs
 * Purpose: Generic base repository — provides standard CRUD for any EF Core entity.
 *          All specific repositories inherit from this class.
 * Who Calls It: All specific repositories (ProductRepository, etc.)
 * Interview Tip: This is the Generic Repository Pattern. It uses C# generics (T)
 *                so one class handles all entity types.
 */

using Microsoft.EntityFrameworkCore;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.Interfaces;

namespace RetailOrderingSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) =>
            await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _dbSet.FindAsync(id) != null;
    }
}
