using FluentValidation;

namespace AgendaApp.Application.Features.Contact.Commands.Validators;

public class DeleteContactValidator : AbstractValidator<DeleteContactCommand>
{
    public DeleteContactValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The Id field is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("The Id field must be a valid GUID.")
                .WithName("Id");
    }
}