using AgendaApp.Application.DTOs.Contact;
using MediatR;

namespace AgendaApp.Application.Features.Contact.Queries;

public class GetContactByIdQuery : IRequest<ContactDto?>
{
    public GetContactByIdQuery(Guid userId ,Guid contactId)
    {
        UserId = userId;
        ContactId = contactId;
    }

    public Guid UserId { get; private set; }
    public Guid ContactId { get; private set; }
}