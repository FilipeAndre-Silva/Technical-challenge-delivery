namespace AgendaApp.Domain.Validators;

public class DomainValidationResult
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool IsValid() => !_errors.Any();

    public IReadOnlyDictionary<string, List<string>> Errors => _errors;

    public void AddError(string field, string message)
    {
        if (!_errors.ContainsKey(field))
            _errors[field] = new List<string>();

        _errors[field].Add(message);
    }

    public IEnumerable<string> GetAllMessages()
    {
        return _errors.SelectMany(e => e.Value.Select(msg => $"{e.Key}: {msg}"));
    }
}