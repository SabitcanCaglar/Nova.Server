using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Features.Auths.Commands.Login;
using IdentityService.Api.Application.Features.Auths.Rules;
using IdentityService.Api.Application.Models.Login;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Identity;
using IdentityService.Api.Domain.Jwt;
using IdentityService.Api.Infrastructure.Repositories;
using Moq;
using Services.Core.Results;

namespace IdentityServivce.UnitTest.Auth.Login
{
    [TestFixture]
    public class LoginCommandTests
    {
        private LoginCommand.LoginCommandHandler _loginCommandHandler;
        private Mock<AuthBusinessRules> _mockAuthBusinessRules;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<AuthManager> _mockAuthService;
        private UserOperationClaimRepository _userOperationClaimRepository;
        private Mock<ITokenHelper> _mockTokenHelper;
        private RefreshTokenRepository _refreshTokenRepository;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = ApplicationDbContextFactory.CreateDbContext();
            _mockIdentityService = new Mock<IIdentityService>();
            _mockAuthBusinessRules = new Mock<AuthBusinessRules>(_mockIdentityService.Object);
            _userOperationClaimRepository = new UserOperationClaimRepository(_context);
            _mockTokenHelper = new Mock<ITokenHelper>();
            _refreshTokenRepository = new RefreshTokenRepository(_context);
            _mockAuthService = new Mock<AuthManager>(_userOperationClaimRepository, _mockTokenHelper.Object, _refreshTokenRepository);
            _loginCommandHandler = new LoginCommand.LoginCommandHandler(_mockIdentityService.Object, _mockAuthService.Object);
        }

        [Test]
        public async Task T_01_handle_valid_request_returns_success_result()
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

            var user = new User()
            {
                Email = requestDto.Email,
            };

            var userDataResult = new SuccessDataResult<User>(user);

            _mockIdentityService.Setup(s => s.GetUserWithEmailAsync(requestDto.Email)).ReturnsAsync(userDataResult);

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

            _mockIdentityService.Verify(s => s.GetUserWithEmailAsync(requestDto.Email), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(userDataResult.Data), Times.Once);
        }

        [Test]
        public async Task T_02_handle_user_not_found_returns_error_result()
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

            _mockIdentityService.Setup(s => s.GetUserWithEmailAsync(requestDto.Email)).ReturnsAsync(userDataResult);

            // Act
            var result = await _loginCommandHandler.Handle(loginCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<ErrorDataResult<LoginResponseDto>>(result);

            var errorResult = (ErrorDataResult<LoginResponseDto>)result;
            Assert.AreEqual("User not found!", errorResult.Message);

            _mockIdentityService.Verify(s => s.GetUserWithEmailAsync(requestDto.Email), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(It.IsAny<User>()), Times.Never);
        }
    }
}
