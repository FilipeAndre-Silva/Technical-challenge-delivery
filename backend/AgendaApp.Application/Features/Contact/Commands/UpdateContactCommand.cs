using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Application.Features.Contact.Commands.Validators;
using System.Text.Json.Serialization;

namespace AgendaApp.Application.Features.Contact.Commands;

public class UpdateContactCommand : CommandBase<UpdateContactCommand, UpdateContactValidator, ContactDto>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonIgnore]
    public Guid UserId { get; private set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public void SetId(Guid contactId)
    {
        Id = contactId;
    }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}