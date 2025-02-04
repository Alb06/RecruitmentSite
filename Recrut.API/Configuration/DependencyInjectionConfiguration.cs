using Recrut.Data.Repositories.Interfaces;
using Recrut.Data.Repositories;

namespace Recrut.API.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
