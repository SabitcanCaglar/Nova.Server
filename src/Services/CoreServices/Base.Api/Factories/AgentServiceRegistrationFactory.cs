using Consul;

namespace Base.Api.Factories;

public abstract class AgentServiceRegistrationFactory
{
    public static AgentServiceRegistration CreateUserAgentServiceRegistration(string serviceName, Uri uri)
    {
        return new AgentServiceRegistration()
        {
            ID = $"{serviceName}Service",
            Name = $"{serviceName}Service",
            Address = $"{uri.Host}",
            Port = uri.Port,
            Tags = new[] { $"{serviceName} Service", serviceName }
        };
    }
}