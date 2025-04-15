namespace AgendaApp.Application.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public Dictionary<string, string> Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message ?? "Operation completed successfully.",
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> Fail(string message, Dictionary<string, string> errors)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }

    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = null
        };
    }
}