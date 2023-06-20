using IdentityService.Api.Application.Models.Login;
using IdentityService.Api.Application.Services.Abstracts;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Repositories;
using MediatR;
using Services.Core.Results;
using Services.Core.Security.Hashing;


namespace IdentityService.Api.Application.Features.Auths.Commands.Login
{
    public class LoginCommand:IRequest<DataResult<LoginResponseDto>>
    {
        public LoginRequestDto LoginRequestDto { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, DataResult<LoginResponseDto>>
        {
            private readonly IIdentityService _identityService;
            private readonly IAuthService _authService;

            public LoginCommandHandler(IIdentityService identityService, IAuthService authService)
            {
                _identityService = identityService;
                _authService = authService;
            }

            public async Task<DataResult<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {

                var userDataResult =await _identityService.GetUserAsync(request.LoginRequestDto.Email);
                
                if (!userDataResult.Success)
                {
                    return new ErrorDataResult<LoginResponseDto>("User not found!");
                }
                
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(request.LoginRequestDto.Password, out passwordHash, out passwordSalt);

                var isVerify = HashingHelper.VerifyPasswordHash(request.LoginRequestDto.Password, passwordHash, passwordSalt);

                if (!isVerify)
                {
                   // return new ErrorDataResult<LoginResponseDto>("Email or password incorrect");
                }
                
                var accessToken = await _authService.CreateAccessToken(userDataResult.Data);
                
                return new SuccessDataResult<LoginResponseDto>( new LoginResponseDto(userDataResult.Data.Email, Guid.Parse(userDataResult.Data.Id), accessToken));
            }
        }
    }
}