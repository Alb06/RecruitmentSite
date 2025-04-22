namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsConfigurations(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", policy =>
                {
                    policy.WithOrigins("https://127.0.0.1:63921", "https://localhost:63921")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));

                    // En développement uniquement, on peut être plus permissif
                    if (env.IsDevelopment())
                    {
                        // Pour le développement uniquement! À ne pas utiliser en production. Utilisez toujours une liste explicite d'origines autorisées
                        policy.SetIsOriginAllowed(_ => true);
                    }
                });
            });
            return services;
        }
    }
}
