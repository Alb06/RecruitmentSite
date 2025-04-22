using Recrut.AppliWeb.Server.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace Recrut.AppliWeb.Server.Services
{
    /// <summary>
    /// Service proxy pour les opérations de gestion des utilisateurs
    /// </summary>
    public class UserProxyService : IUserProxyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProxyService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserProxyService(IHttpClientFactory httpClientFactory, ILogger<UserProxyService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("RecrutAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Ajoute le token JWT à l'en-tête d'autorisation si présent
        /// </summary>
        /// <param name="token">Token JWT</param>
        private void AddAuthorizationHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// Récupère un utilisateur par son email
        /// </summary>
        public async Task<HttpResponseMessage> GetUserByEmailAsync(string email, string token = null)
        {
            try
            {
                AddAuthorizationHeader(token);
                _logger.LogInformation("Récupération de l'utilisateur avec l'email: {Email}", email);
                return await _httpClient.GetAsync($"/api/users/{email}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur par email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Récupère des utilisateurs par leurs IDs
        /// </summary>
        public async Task<HttpResponseMessage> GetUsersByIdsAsync(IEnumerable<int> ids, string token)
        {
            try
            {
                AddAuthorizationHeader(token);

                var queryString = string.Join("&", ids.Select(id => $"ids={id}"));
                _logger.LogInformation("Récupération des utilisateurs avec les IDs: {IDs}", string.Join(", ", ids));

                return await _httpClient.GetAsync($"/api/users/byIds?{queryString}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs par IDs");
                throw;
            }
        }

        /// <summary>
        /// Crée de nouveaux utilisateurs
        /// </summary>
        public async Task<HttpResponseMessage> CreateUsersAsync(object usersDto, string token = null)
        {
            try
            {
                AddAuthorizationHeader(token);
                _logger.LogInformation("Création de nouveaux utilisateurs");

                return await _httpClient.PostAsJsonAsync("/api/users/createUsers", usersDto, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la création des utilisateurs");
                throw;
            }
        }

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        public async Task<HttpResponseMessage> UpdateUserAsync(int id, object userDto, string token)
        {
            try
            {
                AddAuthorizationHeader(token);
                _logger.LogInformation("Mise à jour de l'utilisateur avec l'ID: {ID}", id);

                return await _httpClient.PutAsJsonAsync($"/api/users/{id}", userDto, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur avec l'ID: {ID}", id);
                throw;
            }
        }

        /// <summary>
        /// Supprime un utilisateur par son email
        /// </summary>
        public async Task<HttpResponseMessage> DeleteUserByEmailAsync(string email, string token)
        {
            try
            {
                AddAuthorizationHeader(token);
                _logger.LogInformation("Suppression de l'utilisateur avec l'email: {Email}", email);

                return await _httpClient.DeleteAsync($"/api/users/{email}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur avec l'email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Supprime des utilisateurs par leurs IDs
        /// </summary>
        public async Task<HttpResponseMessage> DeleteUsersByIdsAsync(IEnumerable<int> ids, string token)
        {
            try
            {
                AddAuthorizationHeader(token);
                _logger.LogInformation("Suppression des utilisateurs avec les IDs: {IDs}", string.Join(", ", ids));

                var request = new HttpRequestMessage(HttpMethod.Delete, "/api/users/byIds")
                {
                    Content = new StringContent(JsonSerializer.Serialize(ids, _jsonOptions),
                                                Encoding.UTF8,
                                                "application/json")
                };

                return await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression des utilisateurs par IDs");
                throw;
            }
        }
    }
}
