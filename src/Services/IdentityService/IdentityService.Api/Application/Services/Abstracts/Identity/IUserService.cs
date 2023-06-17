using Base.Domain.Entities.Identity;

namespace IdentityService.Api.Application.Services.Abstracts.Identity
{
    public interface IUserService
    {
        List<OperationClaim> GetClaims(User user);
        void Add(User user);
        User GetByMail(string email);
    }
}
