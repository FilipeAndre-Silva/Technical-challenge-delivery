namespace AgendaApp.Application.DTOs.User;

public class ResetPasswordConfirmationDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}