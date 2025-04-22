namespace Recrut.AppliWeb.Server.Middleware
{
    /// <summary>
    /// Middleware pour déboguer les problèmes CORS
    /// À utiliser uniquement pendant le développement
    /// </summary>
    public class CorsDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorsDebugMiddleware> _logger;

        public CorsDebugMiddleware(RequestDelegate next, ILogger<CorsDebugMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log la requête entrante
            _logger.LogInformation(
                "Requête entrante: {Method} {Path}, Origin: {Origin}, Referer: {Referer}",
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers["Origin"],
                context.Request.Headers["Referer"]);

            // Exécuter le prochain middleware dans le pipeline
            await _next(context);

            // Log la réponse sortante
            _logger.LogInformation(
                "Réponse sortante: {StatusCode}, CORS Headers: Origin: {AccessControlAllowOrigin}, Methods: {AccessControlAllowMethods}, Headers: {AccessControlAllowHeaders}",
                context.Response.StatusCode,
                context.Response.Headers["Access-Control-Allow-Origin"],
                context.Response.Headers["Access-Control-Allow-Methods"],
                context.Response.Headers["Access-Control-Allow-Headers"]);
        }
    }
}