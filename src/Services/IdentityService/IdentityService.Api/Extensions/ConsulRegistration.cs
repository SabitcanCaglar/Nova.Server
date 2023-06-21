using Base.Api.Factories;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Nova.Core.Common;

namespace IdentityService.Api.Extensions;

public static class ConsulRegistration
{
    private const string ServiceName = "Identity";

    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {

            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));
        return services;
    }

    public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

        var addressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
        if (addressesFeature != null && addressesFeature.Addresses.Count > 0)
        {
            var address = addressesFeature.Addresses.First();
            var uri = new Uri(address);
            Register(lifetime, uri, logger, consulClient);
        }
        else
        {
            var defaultAddress = ServiceDefaultUrls.IdentityServiceUrl;
            var uri = new Uri(defaultAddress);
            Register(lifetime, uri, logger, consulClient);
        }
        return app;
    }

    private static void Register(IHostApplicationLifetime lifetime, Uri uri, ILogger<IApplicationBuilder> logger, IConsulClient consulClient)
    {
        var registration = AgentServiceRegistrationFactory.CreateUserAgentServiceRegistration(ServiceName, uri);

        logger.LogInformation("Register with consul!");
        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Unregistering from consul!");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });
    }
}