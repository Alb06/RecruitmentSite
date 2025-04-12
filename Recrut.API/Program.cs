using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Recrut.API.Configuration;
using Recrut.API.Middleware;
using Recrut.Shared.Authentication;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);

// Ajout des configurations
builder.Services.AddDatabaseConfiguration();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddDependencyInjectionConfiguration();
builder.Services.AddSwaggerConfiguration();

// Configuration JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ??
            throw new InvalidOperationException("JWT Secret is not configured")))
    };
});

// Configuration Autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers();

var app = builder.Build();

// Activation des middlewares
app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Activation de l'authentification et de l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
