using Base.Application.Repositories;
using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Services.Repositories;

namespace IdentityService.Api.Infrastructure.Repositories
{
    public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, ApplicationDbContext>,IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

}
