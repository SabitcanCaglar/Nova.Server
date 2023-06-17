using Base.Domain.Entities.Identity;
using Services.Core.Results;
using Result = Base.Application.Models.Result;

namespace IdentityService.Api.Application.Services.Abstracts.Identity;

public interface IIdentityService
{
    Guid UserId { get; set; }
    Task<DataResult<User>> GetUserAsync(string userId);
    Task<DataResult<User>> GetUserWithEmailAsync(string email);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(DataResult<User?> Result, string? ApplicationUserId)> CreateUserAsync(User user, string password);

    Task<Result> DeleteUserAsync(string userId);
    
}