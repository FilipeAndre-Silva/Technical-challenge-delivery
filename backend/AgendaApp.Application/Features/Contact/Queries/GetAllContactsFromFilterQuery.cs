using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AgendaApp.Application.Features.Contact.Queries;

public class GetAllContactsFromFilterQuery : IRequest<PagedResult<ContactDto>>
{
    [BindNever]
    public Guid UserId { get; private set; }

    public string? SearchName { get; set; }
    public bool IsPaged { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }
}