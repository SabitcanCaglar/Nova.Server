using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Features.Auths.Commands.Register;
using IdentityService.Api.Application.Features.Auths.Rules;
using IdentityService.Api.Application.Models.Register;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Identity;
using IdentityService.Api.Application.Services.Repositories;
using IdentityService.Api.Domain.Jwt;
using Moq;
using Services.Core.Results;
using Services.Core.Security.Hashing;

namespace IdentityServivce.UnitTests.Auth.Register
{
    [TestFixture]
    public class RegisterCommandTests
    {
        private RegisterCommand.RegisterCommandHandler _registerCommandHandler;
        private Mock<AuthBusinessRules> _mockAuthBusinessRules;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<IAuthService> _mockAuthService;
        private Mock<IUserOperationClaimRepository> _mockUserOperationClaimRepository;
        private Mock<ITokenHelper> _mockTokenHelper;
        private Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private Mock<ApplicationDbContext> _mockContext;
        
        
        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockIdentityService              = new Mock<IIdentityService>();
            _mockAuthBusinessRules            = new Mock<AuthBusinessRules>(_mockIdentityService);
            _mockUserOperationClaimRepository = new Mock<IUserOperationClaimRepository>();
            _mockTokenHelper                  = new Mock<ITokenHelper>();
            _mockRefreshTokenRepository       = new Mock<IRefreshTokenRepository>();
            
            var authManager = new AuthManager(_mockUserOperationClaimRepository.Object,_mockTokenHelper.Object,_mockRefreshTokenRepository.Object);

            _registerCommandHandler           = new RegisterCommand.RegisterCommandHandler(
                _mockAuthBusinessRules.Object,
                _mockIdentityService.Object,
                authManager
            );
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsRegisteredDto()
        {
            // Arrange
            var requestDto = new RegisterRequestDto
            {
                Email = "test@example.com",
                Password = "password",
                FirstName = "John",
                LastName = "Doe"
            };
            var ipAddress = "127.0.0.1";

            var registerCommand = new RegisterCommand
            {
                RegisterRequestDto = requestDto,
                IpAddress = ipAddress
            };

            var passwordHash = new byte[] { 1, 2, 3 };
            var passwordSalt = new byte[] { 4, 5, 6 };

            HashingHelper.CreatePasswordHash(requestDto.Password, out passwordHash, out passwordSalt);

            var newUser = new User
            {
                Email = requestDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                Status = true,
                UserName = $"{requestDto.FirstName.ToLower()}_{requestDto.LastName.ToLower()}"
            };

            var createdUserResult = new SuccessDataResult<User>(newUser);
            var userId = "12345";

            _mockAuthBusinessRules
                .Setup(r => r.EmailCanNotBeDuplicatedWhenRegistered(requestDto.Email))
                .Returns(Task.CompletedTask);

            _mockIdentityService
                .Setup(s => s.CreateUserAsync(newUser, requestDto.Password))
                .ReturnsAsync((createdUserResult, userId));

            var createdAccessToken = new AccessToken();
            _mockAuthService
                .Setup(s => s.CreateAccessToken(newUser))
                .ReturnsAsync(createdAccessToken);

            var createdRefreshToken = new RefreshToken();
            _mockAuthService
                .Setup(s => s.CreateRefreshToken(newUser, ipAddress))
                .ReturnsAsync(createdRefreshToken);

            _mockAuthService
                .Setup(s => s.AddRefreshToken(createdRefreshToken))
                .ReturnsAsync(createdRefreshToken);

            // Act
            var result = await _registerCommandHandler.Handle(registerCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(createdAccessToken, result.AccessToken);
            Assert.AreEqual(createdRefreshToken, result.RefreshToken);

            _mockAuthBusinessRules.Verify(r => r.EmailCanNotBeDuplicatedWhenRegistered(requestDto.Email), Times.Once);
            _mockIdentityService.Verify(s => s.CreateUserAsync(newUser, requestDto.Password), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(newUser), Times.Once);
            _mockAuthService.Verify(s => s.CreateRefreshToken(newUser, ipAddress), Times.Once);
            _mockAuthService.Verify(s => s.AddRefreshToken(createdRefreshToken), Times.Once);
        }
    }
}
