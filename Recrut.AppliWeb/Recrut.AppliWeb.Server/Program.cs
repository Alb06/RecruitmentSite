using Recrut.AppliWeb.Server.Configuration.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuration CORS
builder.Services.AddCorsConfigurations(builder.Environment);

// Ajouter les services au conteneur DI
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurer les options JSON pour correspondre à l'API
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configuration des clients HTTP pour communiquer avec l'API
builder.Services.AddHttpClients(builder.Configuration);

builder.Services.AddDependencyInjectionConfiguration();

builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure le pipeline de requêtes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseEnvDevelopmentConfig();
}

// IMPORTANT: Appliquer CORS avant d'autres middlewares, notamment UseHttpsRedirection
app.UseCors("DefaultPolicy");

app.UseDefaultFiles();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    // N'activer la redirection HTTPS qu'en production
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();