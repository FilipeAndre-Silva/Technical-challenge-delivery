using AgendaApp.Domain.Entities;
using AgendaApp.Domain.Interfaces.ReadRepository;
using Dapper;
using System.Data;
using System.Text;

namespace AgendaApp.Infrastructure.ReadRepository;

public class ContactReadRepository : IContactReadRepository
{
    private readonly IDbConnection _dbConnection;

    public ContactReadRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<(IEnumerable<Contact> Result, int TotalCount, int TotalPages, int CurrentPage, int RemainingItems, int RemainingPages)>
    GetAllFromFilterAsync(Guid userId, string? searchName, bool isPaged, int pageNumber, int pageSize)
    {
        var baseSql = new StringBuilder(@"
            FROM Contacts
            WHERE DeletedAt IS NULL AND CreatedBy = @UserId
        ");

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            baseSql.Append(" AND LOWER(Name) LIKE LOWER(@SearchName)");
            parameters.Add("SearchName", $"%{searchName}%");
        }

        var countSql = "SELECT COUNT(*) " + baseSql.ToString();
        var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, parameters);

        var totalPages = isPaged ? (int)Math.Ceiling(totalCount / (double)pageSize) : 1;
        var remainingItems = isPaged ? Math.Max(totalCount - (pageNumber * pageSize), 0) : 0;
        var remainingPages = isPaged ? Math.Max(totalPages - pageNumber, 0) : 0;

        var dataSql = new StringBuilder("SELECT Id, Name, Email, Phone, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt, DeletedAt ");
        dataSql.Append(baseSql);
        dataSql.Append(" ORDER BY CreatedAt DESC");

        if (isPaged)
        {
            dataSql.Append(" LIMIT @PageSize OFFSET @Offset");
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", (pageNumber - 1) * pageSize);
        }

        var resultList = await _dbConnection.QueryAsync<Contact>(dataSql.ToString(), parameters);

        return (resultList, totalCount, totalPages, pageNumber, remainingItems, remainingPages);
    }

    public async Task<Contact?> GetByIdAsync(Guid userId, Guid contactId)
    {
        const string sql = @"
        SELECT Id, Name, Email, Phone 
        FROM Contacts 
        WHERE Id = @ContactId AND CreatedBy = @UserId";

        return await _dbConnection.QueryFirstOrDefaultAsync<Contact>(sql, new
        {
            ContactId = contactId,
            UserId = userId
        });
    }
}
