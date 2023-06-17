using Base.Domain.Entities.Identity;
using IdentityService.Api.Domain.Jwt;

namespace IdentityService.Api.Application.Services.Abstracts.Identity
{
    public interface IAuthService
    {
        Task<AccessToken> CreateAccessToken(User user);
        Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
        Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    }
}
