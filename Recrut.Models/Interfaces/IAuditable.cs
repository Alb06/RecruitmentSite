namespace Recrut.Models.Interfaces
{
    /// <summary>
    /// Interface définissant les propriétés d'horodatage pour le suivi des modifications des entités
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Date et heure de création de l'entité (en UTC)
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date et heure de dernière modification de l'entité (en UTC)
        /// Nullable car non définie lors de la création initiale
        /// </summary>
        DateTime? UpdatedAt { get; set; }
    }
}