using IdentityService.Api.Application.Features.Auths.Rules;
using IdentityService.Api.Application.Services.Abstracts.Identity;
using IdentityService.Api.Application.Services.Identity;
using IdentityService.Api.Application.Services.Repositories;
using IdentityService.Api.Domain.Jwt;
using IdentityService.Api.Infrastructure.Repositories;

namespace IdentityService.Api
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenHelper, JwtHelper>();
            services.AddScoped<AuthBusinessRules>();
            services.AddTransient<IIdentityService, IdentityManager>();
            
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
            services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
            
            services.AddScoped<IAuthService, AuthManager>();

            
            return services;
        }
    }
}
