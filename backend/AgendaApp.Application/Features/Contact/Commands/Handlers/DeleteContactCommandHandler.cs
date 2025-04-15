using AgendaApp.Application.Common;
using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AgendaApp.Application.Features.Contact.Commands.Handlers;

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, ApiResponse<bool>>
{
    private readonly IContactRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUserCustom> _userManager;

    public DeleteContactCommandHandler(IContactRepository repository, IUnitOfWork unitOfWork, UserManager<IdentityUserCustom> userManager)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
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

            await _repository.DeleteAsync(request.Id, userExists.Id);
            await _unitOfWork.CommitAsync();

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail("An unexpected error occurred while deleting the contact.");
        }
    }
}