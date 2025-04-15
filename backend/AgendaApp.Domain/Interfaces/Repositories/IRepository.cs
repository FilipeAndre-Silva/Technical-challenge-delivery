using AgendaApp.Domain.Common;

namespace AgendaApp.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<T>> GetAllAsync(Guid userId);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id, Guid userId);
    Task DeleteAllAsync(Guid userId);
}