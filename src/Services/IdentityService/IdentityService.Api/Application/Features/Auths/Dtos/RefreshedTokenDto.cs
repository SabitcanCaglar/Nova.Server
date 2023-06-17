using Base.Domain.Entities.Identity;
using IdentityService.Api.Domain.Jwt;

namespace IdentityService.Api.Application.Features.Auths.Dtos
{
    public class RefreshedTokenDto
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
