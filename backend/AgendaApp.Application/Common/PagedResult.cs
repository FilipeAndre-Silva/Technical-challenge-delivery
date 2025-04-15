namespace AgendaApp.Application.Common;

public class PagedResult<T>
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int RemainingItems { get; set; }
    public int RemainingPages { get; set; }
    public IEnumerable<T> Result { get; set; } = new List<T>();
}