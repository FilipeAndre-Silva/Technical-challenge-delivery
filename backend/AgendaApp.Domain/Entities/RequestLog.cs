namespace AgendaApp.Domain.Entities;

public class RequestLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public int StatusCode { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}