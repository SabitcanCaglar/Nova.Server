using Base.Application.Repositories;
using Base.Domain.Entities.Identity;

namespace IdentityService.Api.Application.Services.Repositories
{
    public interface IRefreshTokenRepository : IAsyncRepository<RefreshToken>, IRepository<RefreshToken>
    {
    }
}
