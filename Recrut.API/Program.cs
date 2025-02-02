using Microsoft.EntityFrameworkCore;
using Recrut.Data;
using Recrut.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Configuration de PostgreSQL avec l'injection de dépendances
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=host.docker.internal;Port=5432;Database=recrutdb;Username=zahagadmin;Password=24rnUZ42")
);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

// Tester la connexion en résolvant `AppDbContext` depuis le conteneur de services
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Users.Add(new User { Name = "Alice", Email = "alice@example.com", PasswordHash = "hashed_password" });
    context.SaveChanges();

    var user = context.Users.FirstOrDefault();
    Console.WriteLine($"User: {user.Name}, Email: {user.Email}");
}

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
