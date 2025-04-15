namespace AgendaApp.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    IContactRepository Contacts { get; }
    Task<int> CommitAsync(); 
    Task RollbackAsync();
}