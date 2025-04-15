using AgendaApp.Application.DTOs.User;
using AgendaApp.Application.Services;
using AgendaApp.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AgendaApp.Tests.Application
{
    public class UserAppServiceTests
    {
        private readonly Mock<UserManager<IdentityUserCustom>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUserCustom>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly UserAppService _service;

        public UserAppServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUserCustom>>();
            _userManagerMock = new Mock<UserManager<IdentityUserCustom>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUserCustom>>();

            _signInManagerMock = new Mock<SignInManager<IdentityUserCustom>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns("minha-chave-super-secreta-de-32-bytes!");
            _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");

            _service = new UserAppService(_userManagerMock.Object, _signInManagerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Register_Success()
        {
            var model = new RegisterDto { Email = "test@email.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUserCustom>(), model.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _service.Register(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("User registered successfully.", result.Message);
        }

        [Fact]
        public async Task Register_Failure()
        {
            var model = new RegisterDto { Email = "fail@email.com", Password = "failpass" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUserCustom>(), model.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "Something went wrong" }));

            var result = await _service.Register(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("User registration failed.", result.Message);
        }

        [Fact]
        public async Task Login_Success()
        {
            var model = new LoginDto { Email = "login@success.com", Password = "pass" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(new IdentityUserCustom { Id = Guid.NewGuid(), Email = model.Email });

            var result = await _service.Login(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("Login successful.", result.Message);
        }

        [Fact]
        public async Task Login_InvalidCredentials()
        {
            var model = new LoginDto { Email = "invalid@email.com", Password = "wrong" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _service.Login(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid login attempt.", result.Message);
        }

        [Fact]
        public async Task ResetPassword_Success()
        {
            var model = new ResetPasswordDto { Email = "reset@email.com" };
            var user = new IdentityUserCustom { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token123");

            var result = await _service.ResetPassword(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("Password reset link generated.", result.Message);
        }

        [Fact]
        public async Task ResetPassword_UserNotFound()
        {
            var model = new ResetPasswordDto { Email = "notfound@email.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((IdentityUserCustom)null);

            var result = await _service.ResetPassword(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordConfirmation_Success()
        {
            var model = new ResetPasswordConfirmationDto { Email = "user@email.com", Token = "token", NewPassword = "NewPass!123" };
            var user = new IdentityUserCustom { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, model.Token, model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _service.ResetPasswordConfirmation(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("Password reset successful.", result.Message);
        }

        [Fact]
        public async Task ResetPasswordConfirmation_Failure()
        {
            var model = new ResetPasswordConfirmationDto { Email = "user@email.com", Token = "token", NewPassword = "fail" };
            var user = new IdentityUserCustom { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, model.Token, model.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "ResetFailed", Description = "Invalid token" }));

            var result = await _service.ResetPasswordConfirmation(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("Password reset failed.", result.Message);
        }

        [Fact]
        public async Task UpdateUserData_Success()
        {
            var model = new UpdateUserDataDto { Email = "old@email.com", NewEmail = "new@email.com" };
            var user = new IdentityUserCustom { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<IdentityUserCustom>())).ReturnsAsync(IdentityResult.Success);

            var result = await _service.UpdateUserData(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("User data updated successfully.", result.Message);
        }

        [Fact]
        public async Task UpdateUserData_UserNotFound()
        {
            var model = new UpdateUserDataDto { Email = "notfound@email.com", NewEmail = "new@email.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((IdentityUserCustom)null);

            var result = await _service.UpdateUserData(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task UpdateUserData_UpdateFails()
        {
            var model = new UpdateUserDataDto { Email = "fail@email.com", NewEmail = "new@email.com" };
            var user = new IdentityUserCustom { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "UpdateFail", Description = "Could not update" }));

            var result = await _service.UpdateUserData(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to update user data.", result.Message);
        }
    }
}
