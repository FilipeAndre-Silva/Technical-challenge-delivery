using AgendaApp.Domain.Entities;

namespace AgendaApp.Domain.Interfaces.ReadRepository
{
    public interface IContactReadRepository
    {
        Task<(IEnumerable<Contact> Result, int TotalCount, int TotalPages, int CurrentPage, int RemainingItems, int RemainingPages)>
            GetAllFromFilterAsync(Guid userId, string? searchName, bool isPaged, int pageNumber, int pageSize);
        Task<Contact?> GetByIdAsync(Guid userId, Guid contactId);
    }
}