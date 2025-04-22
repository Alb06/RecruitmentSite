namespace Recrut.AppliWeb.Server.Configuration.Extensions
{
    /// <summary>
    /// Extensions pour configurer les clients HTTP utilisés pour communiquer avec l'API backend
    /// </summary>
    public static class HttpClientConfiguration
    {
        /// <summary>
        /// Ajoute et configure les clients HTTP pour appeler l'API principale
        /// </summary>
        /// <param name="services">Collection de services de l'application</param>
        /// <param name="configuration">Configuration de l'application</param>
        /// <returns>Collection de services mise à jour</returns>
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            // Récupération des paramètres de configuration
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            var timeout = configuration["ApiSettings:Timeout"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl), "L'URL de base de l'API n'est pas configurée");

            if (!double.TryParse(timeout, out double timeoutSeconds))
                timeoutSeconds = 30; // Valeur par défaut si la configuration est invalide

            // Configuration du client HTTP principal pour l'API
            services.AddHttpClient("RecrutAPI", client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}