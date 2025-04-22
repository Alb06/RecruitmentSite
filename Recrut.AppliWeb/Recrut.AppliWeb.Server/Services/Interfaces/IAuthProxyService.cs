namespace Recrut.AppliWeb.Server.Services.Interfaces
{
    /// <summary>
    /// Interface pour le service proxy d'authentification
    /// </summary>
    public interface IAuthProxyService
    {
        /// <summary>
        /// Authentifie un utilisateur auprès de l'API backend
        /// </summary>
        /// <param name="loginDto">Données de connexion</param>
        /// <returns>Résultat de l'authentification</returns>
        Task<HttpResponseMessage> LoginAsync(object loginDto);
    }
}
