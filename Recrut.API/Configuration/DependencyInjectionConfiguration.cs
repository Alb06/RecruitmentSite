using Recrut.Data.Repositories.Interfaces;
using Recrut.Data.Repositories;
using Recrut.Business.Services.Interfaces;
using Recrut.Business.Services;
using Recrut.API.Mapping;

namespace Recrut.API.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAuthRepository, UserAuthRepository>();

            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
