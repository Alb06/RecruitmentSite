using Microsoft.OpenApi.Models;

namespace Recrut.API.Configuration
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1",
                    Description = "API de gestion du site",
                    Contact = new OpenApiContact
                    {
                        Name = "Support",
                        Email = "support@example.com"
                    }
                });
            });
        }

        public static void UseSwaggerConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
                    c.RoutePrefix = "";
                });
            }
        }
    }
}
