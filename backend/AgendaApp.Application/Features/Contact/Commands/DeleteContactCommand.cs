using AgendaApp.Application.Common;
using AgendaApp.Application.Features.Contact.Commands.Validators;
using System.Text.Json.Serialization;

namespace AgendaApp.Application.Features.Contact.Commands;

public class DeleteContactCommand : CommandBase<DeleteContactCommand, DeleteContactValidator, bool>
{
    public DeleteContactCommand(Guid userId ,Guid id)
    {
        UserId = userId;
        Id = id;
    }

    [JsonIgnore]
    public Guid UserId { get; private set; }

    public Guid Id { get; private set; }
}