using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Infrastructure.Context;

namespace AgendaApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AgendaAppDbContext _context;

    public IContactRepository Contacts { get; }

    public UnitOfWork(AgendaAppDbContext context, IContactRepository contactRepository)
    {
        _context = context;
        Contacts = contactRepository;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public Task RollbackAsync()
    {
        return Task.CompletedTask;
    }
}