using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Domain.Services;
using AgendaApp.Infrastructure.Context;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AgendaApp.Application.Features.Contact.Commands.Handlers;

public class CreateContactHandler : IRequestHandler<CreateContactCommand, ApiResponse<ContactDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUserCustom> _userManager;

    public CreateContactHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<IdentityUserCustom> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<ApiResponse<ContactDto>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
                return request.GetValidationErrorsResponse<ContactDto>();

            var userExists = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (userExists == null)
            {
                request.AddError(r => r.UserId, "The user does not exist.");
                return request.GetResponseWithErrors();
            }

            if (await _unitOfWork.Contacts.ExistsByNameAsync(request.UserId, request.Name))
            {
                request.AddError(r => r.Name, "A contact with this name already exists.");
                return request.GetResponseWithErrors();
            }

            var newContact = new Domain.Entities.Contact(request.Name, request.Email, request.Phone, userExists.Id);

            var domainValidation = ContactDomainService.Validate(newContact);
            if (!domainValidation.IsValid())
            {
                request.AddErrors(domainValidation.Errors);
                return request.GetResponseWithErrors();
            }

            await _unitOfWork.Contacts.AddAsync(newContact);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<ContactDto>(newContact);
            return ApiResponse<ContactDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ContactDto>.Fail("An unexpected error occurred while creating the contact.");
        }
    }
}