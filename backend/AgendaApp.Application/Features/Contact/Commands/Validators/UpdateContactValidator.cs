using FluentValidation;

namespace AgendaApp.Application.Features.Contact.Commands.Validators;

public class UpdateContactValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactValidator()
    {
        RuleFor(x => x.Id)
                .NotEmpty().WithMessage("The Id field is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("The Id field must be a valid GUID.")
                .WithName("Id");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must be at most 100 characters long.")
            .When(x => x.Name != null)
            .WithName("Name");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Please enter a valid Email address.")
            .When(x => x.Email != null)
            .WithName("Email");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone must be at most 20 characters long.")
            .Matches(@"^\+55\s\d{2}\s\d{5}-\d{4}$").WithMessage("Phone must be in the format: +55 81 00000-0000.")
            .When(x => x.Phone != null)
            .WithName("Phone");
    }
}