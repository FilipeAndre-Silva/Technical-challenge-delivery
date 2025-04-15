using FluentValidation;

namespace AgendaApp.Application.Features.Contact.Commands.Validators;

public class DeleteAllContactsValidator : AbstractValidator<DeleteAllContactsCommand>
{
    public DeleteAllContactsValidator()
    {
    }
}