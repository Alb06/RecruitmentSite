namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    public static class EnvironmentDevelopmentExtensions
    {
        public static WebApplication UseEnvDevelopmentConfig(this WebApplication app)
        {
            // Middleware de débogage CORS pour voir ce qui se passe
            app.UseCorsDebugMiddleware();
            app.UseSwaggerConfiguration();

            // Ajout d'un endpoint de diagnostic CORS
            app.MapGet("/cors-test", (HttpContext context) =>
            {
                return Results.Ok(new
                {
                    Message = "CORS fonctionne correctement!",
                    Origin = context.Request.Headers["Origin"].ToString(),
                    Host = context.Request.Host.ToString(),
                    RemoteIP = context.Connection.RemoteIpAddress?.ToString()
                });
            })
            .RequireCors("DefaultPolicy");

            return app;
        }
    }
}
