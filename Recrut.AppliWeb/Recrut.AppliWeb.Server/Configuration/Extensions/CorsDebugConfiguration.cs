using Recrut.AppliWeb.Server.Middleware;

namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    // Extension pour ajouter facilement le middleware
    public static class CorsDebugConfiguration
    {
        public static IApplicationBuilder UseCorsDebugMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsDebugMiddleware>();
        }
    }
}
