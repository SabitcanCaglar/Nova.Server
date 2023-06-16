using Base.Application.Repositories;
using Base.Domain.Entities;
using Base.Domain.Entities.Identity;

namespace IdentityService.Api.Application.Services.Repositories
{
    public interface IOperationClaimRepository : IAsyncRepository<OperationClaim>, IRepository<OperationClaim>
    {
    }
}
