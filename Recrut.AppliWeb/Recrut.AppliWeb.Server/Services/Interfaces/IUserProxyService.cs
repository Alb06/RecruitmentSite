namespace Recrut.AppliWeb.Server.Services.Interfaces
{
    /// <summary>
    /// Interface pour le service proxy de gestion des utilisateurs
    /// </summary>
    public interface IUserProxyService
    {
        /// <summary>
        /// Récupère un utilisateur par son email
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <param name="token">Token JWT optionnel pour l'authentification</param>
        /// <returns>Réponse HTTP avec les données de l'utilisateur</returns>
        Task<HttpResponseMessage> GetUserByEmailAsync(string email, string token = null);

        /// <summary>
        /// Récupère des utilisateurs par leurs IDs
        /// </summary>
        /// <param name="ids">Liste d'IDs d'utilisateurs</param>
        /// <param name="token">Token JWT pour l'authentification</param>
        /// <returns>Réponse HTTP avec la liste des utilisateurs</returns>
        Task<HttpResponseMessage> GetUsersByIdsAsync(IEnumerable<int> ids, string token);

        /// <summary>
        /// Crée de nouveaux utilisateurs
        /// </summary>
        /// <param name="usersDto">Données des utilisateurs à créer</param>
        /// <param name="token">Token JWT optionnel pour l'authentification</param>
        /// <returns>Réponse HTTP avec le résultat de l'opération</returns>
        Task<HttpResponseMessage> CreateUsersAsync(object usersDto, string token = null);

        /// <summary>
        /// Met à jour un utilisateur
        /// </summary>
        /// <param name="id">ID de l'utilisateur</param>
        /// <param name="userDto">Données de l'utilisateur à mettre à jour</param>
        /// <param name="token">Token JWT pour l'authentification</param>
        /// <returns>Réponse HTTP avec le résultat de l'opération</returns>
        Task<HttpResponseMessage> UpdateUserAsync(int id, object userDto, string token);

        /// <summary>
        /// Supprime un utilisateur par son email
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <param name="token">Token JWT pour l'authentification</param>
        /// <returns>Réponse HTTP avec le résultat de l'opération</returns>
        Task<HttpResponseMessage> DeleteUserByEmailAsync(string email, string token);

        /// <summary>
        /// Supprime des utilisateurs par leurs IDs
        /// </summary>
        /// <param name="ids">Liste d'IDs d'utilisateurs</param>
        /// <param name="token">Token JWT pour l'authentification</param>
        /// <returns>Réponse HTTP avec le résultat de l'opération</returns>
        Task<HttpResponseMessage> DeleteUsersByIdsAsync(IEnumerable<int> ids, string token);
    }
}
