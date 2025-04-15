using AgendaApp.Domain.Entities;
using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AgendaApp.Infrastructure.Repositories;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    private readonly AgendaAppDbContext _context;

    public ContactRepository(AgendaAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(Guid userId, string name)
    {
        return await _context.Contacts
            .AsNoTracking()
            .AnyAsync(c => c.CreatedBy == userId && c.Name == name && c.DeletedAt == null);
    }
}
