using AgendaApp.Domain.Entities;

namespace AgendaApp.Domain.Interfaces.Repositories;

public interface IContactRepository : IRepository<Contact>
{
    Task<bool> ExistsByNameAsync(Guid userId, string name);
}