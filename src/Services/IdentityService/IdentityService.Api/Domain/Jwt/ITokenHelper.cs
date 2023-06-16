using Base.Domain.Entities;
using Base.Domain.Entities.Identity;

namespace IdentityService.Api.Domain.Jwt
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);

        RefreshToken CreateRefreshToken(User user, string ipAddress);
    }
}
