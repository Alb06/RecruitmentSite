using Recrut.API.Configuration;
using Recrut.API.Middleware;

var builder = WebApplication.CreateSlimBuilder(args);

// Ajout des configurations
builder.Services.AddDatabaseConfiguration();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddDependencyInjectionConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Activation des middlewares
app.UseSwaggerConfiguration();
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
