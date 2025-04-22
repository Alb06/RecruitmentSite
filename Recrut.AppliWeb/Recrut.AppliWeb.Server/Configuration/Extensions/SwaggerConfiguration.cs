using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(
            //c => {
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "Recrut SPA server",
            //        Version = "v1",
            //        Description = "User management SPA server",
            //        Contact = new OpenApiContact
            //        {
            //            Name = "Support",
            //            Email = "support@example.com"
            //        }
            //    });

            //    // Inclusion des commentaires XML pour la documentation Swagger
            //    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            //    c.IncludeXmlComments(xmlPath);
            //}
            );
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Recrut SPA server v1");
                    c.RoutePrefix = "";
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    c.DefaultModelsExpandDepth(-1); // Cache les modèles par défaut
                });
            }
            return app;
        }
    }
}