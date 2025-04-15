using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Application.Features.Contact.Queries;
using AgendaApp.Domain.Interfaces.ReadRepository;
using AutoMapper;
using MediatR;

namespace AgendaApp.Application.Features.Contact.Handlers;

public class ContactQueryHandler : IRequestHandler<GetAllContactsFromFilterQuery, PagedResult<ContactDto>>,
                                   IRequestHandler<GetContactByIdQuery, ContactDto?>
{
    private readonly IContactReadRepository _repository;
    private readonly IMapper _mapper;

    public ContactQueryHandler(IContactReadRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContactDto>> Handle(GetAllContactsFromFilterQuery request, CancellationToken cancellationToken)
    {
        var contactListWithPagination = await _repository.GetAllFromFilterAsync(request.UserId, request.SearchName, request.IsPaged, request.PageNumber, request.PageSize);

        var dtoList = _mapper.Map<IEnumerable<ContactDto>>(contactListWithPagination.Result);

        return new PagedResult<ContactDto>
        {
            TotalCount = contactListWithPagination.TotalCount,
            TotalPages = contactListWithPagination.TotalPages,
            CurrentPage = contactListWithPagination.CurrentPage,
            RemainingItems = contactListWithPagination.RemainingItems,
            RemainingPages = contactListWithPagination.RemainingPages,
            Result = dtoList
        };
    }

    public async Task<ContactDto?> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.ContactId == null || request.ContactId == Guid.Empty)
            return null;

        var contact = await _repository.GetByIdAsync(request.UserId, request.ContactId);

        if (contact == null)
            return null;

        return _mapper.Map<ContactDto>(contact);
    }
}