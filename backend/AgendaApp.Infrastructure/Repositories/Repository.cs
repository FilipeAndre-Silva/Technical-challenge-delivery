using AgendaApp.Domain.Common;
using AgendaApp.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _dbSet
            .Where(e => e.Id == id && e.CreatedBy == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Guid userId)
    {
        return await _dbSet
            .Where(e => e.CreatedBy == userId)
            .ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var entity = await GetByIdAsync(id, userId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task DeleteAllAsync(Guid userId)
    {
        var allEntities = await _dbSet
            .Where(e => e.CreatedBy == userId)
            .ToListAsync();

        _dbSet.RemoveRange(allEntities);
    }
}
