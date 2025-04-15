using FluentValidation;

namespace AgendaApp.Application.Features.Contact.Commands.Validators;

public class CreateContactValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The Name field is required.")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters long.")
            .WithName("Name");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The Email field is required.")
            .EmailAddress().WithMessage("Please enter a valid Email address.")
            .WithName("Email");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("The Phone field is required.")
            .MaximumLength(20).WithMessage("Phone must be at most 20 characters long.")
            .Matches(@"^\+55\s\d{2}\s\d{5}-\d{4}$").WithMessage("Phone must be in the format: +55 81 00000-0000.")
            .WithName("Phone");
    }
}