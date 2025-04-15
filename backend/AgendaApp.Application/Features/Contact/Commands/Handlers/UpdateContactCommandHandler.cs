using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Domain.Services;
using AgendaApp.Infrastructure.Context;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AgendaApp.Application.Features.Contact.Commands.Handlers;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ApiResponse<ContactDto>>
{
    private readonly IContactRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUserCustom> _userManager;

    public UpdateContactCommandHandler(IContactRepository repository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUserCustom> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ApiResponse<ContactDto>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
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

            var contact = await _repository.GetByIdAsync(request.Id, userExists.Id);
            if (contact == null)
            {
                request.AddError(r => r.Id, "A contact with this name already exists.");
                return request.GetResponseWithErrors();
            }

            if (contact.Name != request.Name && await _unitOfWork.Contacts.ExistsByNameAsync(request.UserId, request.Name))
            {
                request.AddError(r => r.Name, "A contact with this name already exists.");
                return request.GetResponseWithErrors();
            }

            contact.Update(request.Name, request.Email, request.Phone, userExists.Id);

            var domainValidation = ContactDomainService.Validate(contact);
            if (!domainValidation.IsValid())
            {
                request.AddErrors(domainValidation.Errors);
                return request.GetResponseWithErrors();
            }

            await _repository.UpdateAsync(contact);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<ContactDto>(contact);
            return ApiResponse<ContactDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ContactDto>.Fail("An unexpected error occurred while updating the contact.");
        }
    }
}