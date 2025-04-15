using AgendaApp.Application.Common;
using AgendaApp.Domain.Interfaces.Repositories;
using MediatR;

namespace AgendaApp.Application.Features.Contact.Commands.Handlers;

public class DeleteAllContactsCommandHandler : IRequestHandler<DeleteAllContactsCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAllContactsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteAllContactsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.Contacts.DeleteAllAsync(request.UserId);
            await _unitOfWork.CommitAsync();

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail("An unexpected error occurred while deleting all contacts.");
        }
    }
}