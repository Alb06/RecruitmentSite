using Recrut.AppliWeb.Server.Services;
using Recrut.AppliWeb.Server.Services.Interfaces;

namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            // Ajouter les services de proxy pour communiquer avec l'API
            services.AddScoped<IAuthProxyService, AuthProxyService>();
            services.AddScoped<IUserProxyService, UserProxyService>();
        }
    }
}
