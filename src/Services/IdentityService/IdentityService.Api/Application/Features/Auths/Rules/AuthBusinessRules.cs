using Base.Application.Common.Exceptions;
using Base.Domain.Entities;
using Base.Domain.Entities.Identity;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Repositories;

namespace IdentityService.Api.Application.Features.Auths.Rules
{
    public class AuthBusinessRules
    {
        private readonly IIdentityService _identityService;

        public AuthBusinessRules(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public virtual async Task EmailCanNotBeDuplicatedWhenRegistered(string email)
        {
            var dataResult = await _identityService.GetUserWithEmailAsync(email);
            if (dataResult.Data != null) throw new BusinessException("Mail already exists");
        }
    }
}
