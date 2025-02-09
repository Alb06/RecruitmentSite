using Recrut.Models;
using System.Text.Json.Serialization;

namespace Recrut.API.Configuration
{
    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(User[]))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
        
    }
}
