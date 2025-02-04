using Recrut.Models;
using System.Text.Json.Serialization;

namespace Recrut.API.Configuration
{
    //[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    //[JsonSerializable(typeof(User[]))]
    //[JsonSerializable(typeof(Product[]))] // Ajoute d'autres entités ici
    //[JsonSerializable(typeof(Order[]))]
    //internal partial class AppJsonSerializerContext : JsonSerializerContext
    //{
    //}


    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(User[]))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
        
    }
}
