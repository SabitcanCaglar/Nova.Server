using Base.Application.Repositories;
using Base.Domain.Entities;
using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Services.Repositories;

namespace IdentityService.Api.Infrastructure.Repositories
{
    public class UserOperationClaimRepository : EfRepositoryBase<UserOperationClaim, ApplicationDbContext>, IUserOperationClaimRepository
    {
        public UserOperationClaimRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

}
