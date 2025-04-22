using Recrut.AppliWeb.Server.Services.Interfaces;

namespace Recrut.AppliWeb.Server.Services
{
    /// <summary>
    /// Service proxy pour les opérations d'authentification
    /// </summary>
    public class AuthProxyService : IAuthProxyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthProxyService> _logger;

        public AuthProxyService(IHttpClientFactory httpClientFactory, ILogger<AuthProxyService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("RecrutAPI");
            _logger = logger;
        }

        /// <summary>
        /// Authentifie un utilisateur auprès de l'API backend
        /// </summary>
        /// <param name="loginDto">Données de connexion</param>
        /// <returns>Résultat de l'authentification avec token JWT</returns>
        public async Task<HttpResponseMessage> LoginAsync(object loginDto)
        {
            try
            {
                _logger.LogInformation("Envoi de la requête d'authentification à l'API");
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de l'authentification: {StatusCode}", response.StatusCode);
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erreur lors de la communication avec l'API d'authentification");
                throw;
            }
        }
    }
}
