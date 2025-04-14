using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Recrut.API.IntegrationTests
{
    /// <summary>
    /// Extensions pour HttpClient pour faciliter les tests d'API
    /// </summary>
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Effectue une requête POST avec un contenu JSON
        /// </summary>
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            return httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Effectue une requête PUT avec un contenu JSON
        /// </summary>
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var serializedData = JsonSerializer.Serialize(data);
            var content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            return httpClient.PutAsync(url, content);
        }

        /// <summary>
        /// Désérialise le contenu d'une réponse HTTP en un objet du type spécifié
        /// </summary>
        public static async Task<T?> ReadFromJsonAsync<T>(this HttpContent content)
        {
            var stringContent = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(stringContent, JsonOptions);
        }

        /// <summary>
        /// Configure l'en-tête d'autorisation avec un token JWT
        /// </summary>
        public static void AddJwtToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}