using IdentityService.Api.Domain.Jwt;

namespace IdentityService.Api.Application.Models.Login;

public class LoginResponseDto
{
    public LoginResponseDto(string userEmail, Guid userId, AccessToken accessToken)
    {
        Email  = userEmail;
        UserId = userId;
        Token  = accessToken;
    }

    public string Email { get; set; }
    public Guid    UserId { get; set; }
    public AccessToken Token { get; set; }
}