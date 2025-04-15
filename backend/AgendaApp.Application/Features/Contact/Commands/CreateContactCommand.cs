using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Application.Features.Contact.Commands.Validators;
using System.Text.Json.Serialization;

namespace AgendaApp.Application.Features.Contact.Commands;

public class CreateContactCommand : CommandBase<CreateContactCommand, CreateContactValidator, ContactDto>
{
    [JsonIgnore]
    public Guid UserId { get; private set; }

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}