using Base.Domain.Entities.Identity;
using IdentityService.Api.Application.Features.Auths.Commands.Login;
using IdentityService.Api.Application.Models.Login;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Domain.Jwt;
using Moq;
using Services.Core.Results;

namespace IdentityServivce.UnitTest.Auth.Login
{
    [TestFixture]
    public class LoginCommandTests
    {
        private LoginCommand.LoginCommandHandler _loginCommandHandler;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<IAuthService> _mockAuthService;

        [SetUp]
        public void Setup()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockAuthService = new Mock<IAuthService>();

            _loginCommandHandler = new LoginCommand.LoginCommandHandler(_mockIdentityService.Object, _mockAuthService.Object);
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var loginCommand = new LoginCommand
            {
                LoginRequestDto = requestDto,
            };

            var userDataResult = new SuccessDataResult<User>(new User());

            _mockIdentityService.Setup(s => s.GetUserAsync(requestDto.Email)).ReturnsAsync(userDataResult);

            var accessToken = new AccessToken();
            _mockAuthService.Setup(s => s.CreateAccessToken(userDataResult.Data)).ReturnsAsync(accessToken);

            // Act
            var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<SuccessDataResult<LoginResponseDto>>(result);

            var successResult = (SuccessDataResult<LoginResponseDto>)result;
            Assert.AreEqual(requestDto.Email, successResult.Data.Email);
            Assert.AreEqual(accessToken, successResult.Data.Token);

            _mockIdentityService.Verify(s => s.GetUserAsync(requestDto.Email), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(userDataResult.Data), Times.Once);
        }

        [Test]
        public async Task Handle_UserNotFound_ReturnsErrorResult()
        {
            // Arrange
            var requestDto = new LoginRequestDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var loginCommand = new LoginCommand
            {
                LoginRequestDto = requestDto,
            };

            var userDataResult = new ErrorDataResult<User>("User not found!");

            _mockIdentityService.Setup(s => s.GetUserAsync(requestDto.Email)).ReturnsAsync(userDataResult);

            // Act
            var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<ErrorDataResult<LoginResponseDto>>(result);

            var errorResult = (ErrorDataResult<LoginResponseDto>)result;
            Assert.AreEqual("User not found!", errorResult.Message);

            _mockIdentityService.Verify(s => s.GetUserAsync(requestDto.Email), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(It.IsAny<User>()), Times.Never);
        }
    }
}
