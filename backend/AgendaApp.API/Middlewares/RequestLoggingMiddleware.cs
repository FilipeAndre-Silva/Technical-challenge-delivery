using System.Diagnostics;
using System.Security.Claims;
using AgendaApp.Domain.Entities;
using AgendaApp.Infrastructure.Context;

namespace AgendaApp.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AgendaAppDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;

        if (request.Method != HttpMethods.Post &&
            request.Method != HttpMethods.Put &&
            request.Method != HttpMethods.Patch &&
            request.Method != HttpMethods.Delete)
        {
            await _next(context);
            return;
        }
        
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var method = request.Method;
        var path = request.Path;
        var ipAddress = GetClientIp(context);
        var userAgent = request.Headers["User-Agent"].ToString();

        await _next(context);

        stopwatch.Stop();

        var log = new RequestLog
        {
            UserId = userId,
            Method = method,
            Path = path,
            StatusCode = context.Response.StatusCode,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        try
        {
            dbContext.RequestLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving request log to the database.");
        }
    }

    private string GetClientIp(HttpContext context)
    {
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',').First().Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "IP not found";
    }
}