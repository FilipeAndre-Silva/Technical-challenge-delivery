using AgendaApp.Domain.Entities;
using AgendaApp.Domain.Validators;

namespace AgendaApp.Domain.Services;

public static class ContactDomainService
{
    public static DomainValidationResult Validate(Contact contact)
    {
        var result = new DomainValidationResult();

        var email = contact.Email?.Trim().ToLower();
        var emailDomain = email?.Split('@').LastOrDefault();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            result.AddError("Email", "E-mail inválido.");

        if (emailDomain is not null && IsPersonalEmailDomain(emailDomain))
            result.AddError("Email", "E-mails pessoais não são permitidos.");

        return result;
    }

    private static bool IsPersonalEmailDomain(string domain)
    {
        return domain.EndsWith("yahoo.com", StringComparison.OrdinalIgnoreCase);
    }
}