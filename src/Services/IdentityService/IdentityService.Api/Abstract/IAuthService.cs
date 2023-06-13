using Azure.Core;
using IdentityService.Api.Domain.Entities;
using RefreshToken = Duende.IdentityServer.Models.RefreshToken;

namespace IdentityService.Api.Abstract;

public interface IAuthService
{
    public Task<AccessToken> CreateAccessToken(User user);
    public Task<RefreshToken> CreateRefreshToken(User user, string ipAddress);
    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
}