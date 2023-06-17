using Base.Application.Repositories;
using Base.Domain.Entities;
using Base.Domain.Entities.Identity;
using Base.Infrastructure;
using IdentityService.Api.Application.Services.Repositories;

namespace IdentityService.Api.Infrastructure.Repositories
{
    public class OperationClaimRepository : EfRepositoryBase<OperationClaim, ApplicationDbContext>, IOperationClaimRepository
    {
        public OperationClaimRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

}
