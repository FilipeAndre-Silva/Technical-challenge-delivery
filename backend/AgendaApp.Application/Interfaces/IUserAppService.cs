using AgendaApp.Application.DTOs.User;
using AgendaApp.Application.Common;
using AgendaApp.Infrastructure.Context;

namespace AgendaApp.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<ApiResponse<IdentityUserCustom>> Register(RegisterDto model);
        Task<ApiResponse<string>> Login(LoginDto model);
        Task<ApiResponse<string>> ResetPassword(ResetPasswordDto model);
        Task<ApiResponse<string>> ResetPasswordConfirmation(ResetPasswordConfirmationDto model);
        Task<ApiResponse<IdentityUserCustom>> UpdateUserData(UpdateUserDataDto model);
    }
}