using AgendaApp.Application.Common;
using AgendaApp.Application.Features.Contact.Commands.Validators;
using System.Text.Json.Serialization;

namespace AgendaApp.Application.Features.Contact.Commands;

public class DeleteAllContactsCommand : CommandBase<DeleteAllContactsCommand, DeleteAllContactsValidator, bool>
{
    public DeleteAllContactsCommand(Guid userId)
    {
        UserId = userId;
    }

    [JsonIgnore]
    public Guid UserId { get; private set; }
}