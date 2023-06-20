using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Features.Auths.Commands.Register;
using IdentityService.Api.Application.Features.Auths.Rules;
using IdentityService.Api.Application.Models.Register;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Identity;
using IdentityService.Api.Domain.Jwt;
using IdentityService.Api.Infrastructure.Repositories;
using Moq;
using Services.Core.Results;
using Services.Core.Security.Hashing;

namespace IdentityServivce.UnitTest.Auth.Register
{
    [TestFixture]
    public class RegisterCommandTests
    {
        private RegisterCommand.RegisterCommandHandler _registerCommandHandler;
        private Mock<AuthBusinessRules> _mockAuthBusinessRules;
        private Mock<IIdentityService> _mockIdentityService;
        private Mock<AuthManager> _mockAuthService;
        private UserOperationClaimRepository _userOperationClaimRepository;
        private Mock<ITokenHelper> _mockTokenHelper;
        private RefreshTokenRepository _refreshTokenRepository;
        private ApplicationDbContext _context;
        private User _newUser;

        private const string UnitTestRequestUserMail = "test@example.com";
        private const string UnitTestExistUserMail = "exist@example.com";

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

            _registerCommandHandler = new RegisterCommand.RegisterCommandHandler(
                _mockAuthBusinessRules.Object,
                _mockIdentityService.Object,
                _mockAuthService.Object
            );
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsRegisteredDto()
        {
            // Arrange
            var requestDto = new RegisterRequestDto
            {
                Email = UnitTestRequestUserMail,
                Password = "password",
                FirstName = "John",
                LastName = "Doe"
            };

            var registerCommand = new RegisterCommand
            {
                RegisterRequestDto = requestDto,
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

            _mockIdentityService.Setup(i => i.CreateUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync((createdUserResult, createdUserResult.Data.Id));
            
            var createdAccessToken = new AccessToken();
            _mockAuthService
                .Setup(s => s.CreateAccessToken(newUser))
                .ReturnsAsync(createdAccessToken);

            var createdRefreshToken = new RefreshToken();
            _mockAuthService
                .Setup(s => s.CreateRefreshToken(newUser, It.IsAny<string>()))
                .ReturnsAsync(createdRefreshToken);

            _mockAuthService
                .Setup(s => s.AddRefreshToken(createdRefreshToken))
                .ReturnsAsync(createdRefreshToken);

            // Act
            var result = await _registerCommandHandler.Handle(registerCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(createdAccessToken, result.Data.AccessToken);
            Assert.AreEqual(createdRefreshToken, result.Data.RefreshToken);

            _mockAuthBusinessRules.Verify(r => r.EmailCanNotBeDuplicatedWhenRegistered(requestDto.Email), Times.Once);
            _mockAuthService.Verify(s => s.CreateAccessToken(newUser), Times.Once);
            _mockAuthService.Verify(s => s.AddRefreshToken(createdRefreshToken), Times.Once);
        }
    }
}
