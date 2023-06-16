using Base.Domain.Entities.Identity;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Core.Results;
using Result = Base.Application.Models.Result;

namespace IdentityService.Api.Application.Services.Identity;

public class IdentityManager : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityManager(
        UserManager<User> userManager,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager                = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService       = authorizationService;
    }

    public Guid UserId { get; set; }

    public async Task<DataResult<User>> GetUserAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return new SuccessDataResult<User>(user);
    }
    public async Task<DataResult<User>> GetUserWithEmailAsync(string email)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

        return new SuccessDataResult<User>(user);
    }
    public async Task<(DataResult<User?> Result, string ApplicationUserId)> CreateUserAsync(User user, string password)
    {
        
        var result = await _userManager.CreateAsync(user, password);

        if(result.Succeeded)
            return (new SuccessDataResult<User>(user), user.Id);
        else
        {
            return ( new ErrorDataResult<User?>(result.Errors.FirstOrDefault().ToString()), null);
        }
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(User user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
