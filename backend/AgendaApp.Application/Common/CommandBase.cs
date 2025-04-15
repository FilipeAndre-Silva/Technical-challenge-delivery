using FluentValidation;
using MediatR;
using System.Linq.Expressions;

namespace AgendaApp.Application.Common;

public abstract class CommandBase<TCommand, TValidator, TResponse> : IRequest<ApiResponse<TResponse>>
    where TCommand : class
    where TValidator : AbstractValidator<TCommand>, new()
{
    private readonly Dictionary<string, string> Errors = new();

    public bool IsValid()
    {
        var validator = new TValidator();
        var result = validator.Validate(this as TCommand);
        return result.IsValid;
    }

    public ApiResponse<TResponse> GetValidationErrorsResponse<TResponse>(string msg)
    {
        var validator = new TValidator();
        var validation = validator.Validate(this as TCommand);

        if (validation.IsValid)
        {
            return ApiResponse<TResponse>.Success(default, null);
        }

        var errorDictionary = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => string.Join(" | ", g.Select(e => e.ErrorMessage))
            );

        return ApiResponse<TResponse>.Fail(msg, errorDictionary);
    }

    public ApiResponse<TResponse> GetValidationErrorsResponse<TResponse>()
    {
        var validator = new TValidator();
        var validation = validator.Validate(this as TCommand);

        if (validation.IsValid)
        {
            return ApiResponse<TResponse>.Success(default, null);
        }

        var errorDictionary = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => string.Join(" | ", g.Select(e => e.ErrorMessage))
            );

        return ApiResponse<TResponse>.Fail("Validation failed for input data.", errorDictionary);
    }

    public void AddError<TProperty>(Expression<Func<TCommand, TProperty>> propertyExpression, string errorMessage)
    {
        var propertyName = GetPropertyName(propertyExpression);

        if (Errors.ContainsKey(propertyName))
            Errors[propertyName] += " | " + errorMessage;
        else
            Errors[propertyName] = errorMessage;
    }

    public void AddErrors(IReadOnlyDictionary<string, List<string>> errors)
    {
        foreach (var kvp in errors)
        {
            var combinedMessage = string.Join(" | ", kvp.Value);

            if (Errors.ContainsKey(kvp.Key))
                Errors[kvp.Key] += " | " + combinedMessage;
            else
                Errors[kvp.Key] = combinedMessage;
        }
    }

    public ApiResponse<TResponse> GetResponseWithErrors(string message = "Validation failed")
    {
        return Errors.Any()
            ? ApiResponse<TResponse>.Fail(message, Errors)
            : ApiResponse<TResponse>.Success(default, null);
    }

    private string GetPropertyName<TProperty>(Expression<Func<TCommand, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expression is not a valid member expression", nameof(propertyExpression));
    }
}