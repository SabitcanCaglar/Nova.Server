using Application.Features.Auths.Dtos;
using Base.Domain.Entities;
using Base.Domain.Entities.Identity;
using IdentityService.Api.Application.Features.Auths.Rules;
using IdentityService.Api.Application.Models.Register;
using IdentityService.Api.Application.Services.Abstracts;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Repositories;
using IdentityService.Api.Domain.Jwt;
using MediatR;
using Services.Core.Security.Hashing;

namespace IdentityService.Api.Application.Features.Auths.Commands.Register
{
    public class RegisterCommand:IRequest<RegisteredDto>
    {
        public RegisterRequestDto RegisterRequestDto { get; set; }
        public string IpAddress { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisteredDto>
        {
            private readonly AuthBusinessRules _authBusinessRules;
            private readonly IIdentityService _identityService;
            private readonly IAuthService _authService;

            public RegisterCommandHandler(AuthBusinessRules authBusinessRules, IIdentityService identityService, IAuthService authService)
            {
                _authBusinessRules = authBusinessRules;
                _identityService = identityService;
                _authService = authService;
            }

            public async Task<RegisteredDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                await _authBusinessRules.EmailCanNotBeDuplicatedWhenRegistered(request.RegisterRequestDto.Email);
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(request.RegisterRequestDto.Password, out passwordHash, out passwordSalt);

                User newUser = new()
                {
                    Email = request.RegisterRequestDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FirstName = request.RegisterRequestDto.FirstName,
                    LastName = request.RegisterRequestDto.LastName,
                    Status = true,
                    UserName = $"{request.RegisterRequestDto.FirstName.ToLower()}_{request.RegisterRequestDto.LastName.ToLower()}"
                };

                 var (createdUserResult, userId) = await _identityService.CreateUserAsync(newUser,request.RegisterRequestDto.Password.ToString());

                 
                 // Add create  User Id log
                 
                 
                AccessToken createdAccessToken = await _authService.CreateAccessToken(createdUserResult.Data);
                RefreshToken createdRefreshToken = await _authService.CreateRefreshToken(createdUserResult.Data, request.IpAddress);
                
                createdRefreshToken.CreatedByIp = "1";
                
                RefreshToken addedRefreshToken = await _authService.AddRefreshToken(createdRefreshToken);

                RegisteredDto registeredDto = new()
                {
                    RefreshToken = addedRefreshToken,
                    AccessToken = createdAccessToken,
                };
                return registeredDto;

            }
        }
    }
}