using AgendaApp.Application.Common;
using AgendaApp.Application.DTOs.User;
using AgendaApp.Application.Interfaces;
using AgendaApp.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgendaApp.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly UserManager<IdentityUserCustom> _userManager;
    private readonly SignInManager<IdentityUserCustom> _signInManager;
    private readonly IConfiguration _configuration;

    public UserAppService(
        UserManager<IdentityUserCustom> userManager,
        SignInManager<IdentityUserCustom> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<ApiResponse<IdentityUserCustom>> Register(RegisterDto model)
    {
        var user = new IdentityUserCustom { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => e.Description);
            return ApiResponse<IdentityUserCustom>.Fail("User registration failed.", errors);
        }

        return ApiResponse<IdentityUserCustom>.Success(user, "User registered successfully.");
    }

    public async Task<ApiResponse<string>> Login(LoginDto model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Invalid login attempt.");

        var token = await GerarToken(model.Email);
        return ApiResponse<string>.Success(token, "Login successful.");
    }

    public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return ApiResponse<string>.Fail("User not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: send token by email

        return ApiResponse<string>.Success(token, "Password reset link generated.");
    }

    public async Task<ApiResponse<string>> ResetPasswordConfirmation(ResetPasswordConfirmationDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return ApiResponse<string>.Fail("User not found.");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => e.Description);
            return ApiResponse<string>.Fail("Password reset failed.", errors);
        }

        return ApiResponse<string>.Success(null, "Password reset successful.");
    }

    public async Task<ApiResponse<IdentityUserCustom>> UpdateUserData(UpdateUserDataDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return ApiResponse<IdentityUserCustom>.Fail("User not found.");

        user.UserName = model.NewEmail ?? user.UserName;
        user.Email = model.NewEmail ?? user.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => e.Description);
            return ApiResponse<IdentityUserCustom>.Fail("Failed to update user data.", errors);
        }

        return ApiResponse<IdentityUserCustom>.Success(null, "User data updated successfully.");
    }

    private async Task<string> GerarToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credenciais,
            claims: claims
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}