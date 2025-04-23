# Ajout de fonctionnalités d'audit aux entités

## Comment implémenter l'interface IAuditable dans une nouvelle entité

Pour ajouter les fonctionnalités d'audit (suivi des dates de création et de modification) à une nouvelle entité, suivez ces étapes :

1. Assurez-vous que votre classe implémente l'interface `IAuditable` :

```csharp
using Recrut.Models.Interfaces;
using System;

namespace Recrut.Models
{
    public class NouvelleEntite : IEntity, IAuditable
    {
        // Propriétés de l'entité...
        public int Id { get; set; }
        public required string Attribut1 { get; set; }
        public required string Attribut2 { get; set; }
        
        // Propriétés d'audit requises par l'interface IAuditable
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
```

2. Ajoutez la configuration dans la méthode `OnModelCreating` de `AppDbContext` (facultatif, mais recommandé) :

```csharp
// Dans la méthode OnModelCreating de AppDbContext
modelBuilder.Entity<NouvelleEntite>()
    .Property(e => e.CreatedAt)
    .HasColumnType("timestamp with time zone")
    .IsRequired();
    
modelBuilder.Entity<NouvelleEntite>()
    .Property(e => e.UpdatedAt)
    .HasColumnType("timestamp with time zone")
    .IsRequired(false);
```

3. Créez une migration pour ajouter la nouvelle entité à la base de données :

```bash
dotnet ef migrations add AddNouvelleEntite -p Recrut.Data -s Recrut.Data
dotnet ef database update -p Recrut.Data -s Recrut.Data
```

## Comportement attendu

- Lorsqu'une entité est créée, la propriété `CreatedAt` est automatiquement initialisée avec la date et l'heure actuelles en UTC
- Lorsqu'une entité est modifiée via la méthode `UpdateAsync` du repository, la propriété `UpdatedAt` est automatiquement mise à jour avec la date et l'heure actuelles en UTC
- Ces informations sont persistées dans la base de données et peuvent être utilisées pour l'audit et le suivi des modifications

# Suppression de migrations Entity Framework Core

## Migration créée mais pas appliquée
Pour supprimer un fichier de migration non appliqué :
```bash
dotnet ef migrations remove -p Recrut.Data -s Recrut.Data
```

## Migration déjà appliquée en base
Deux étapes sont nécessaires :

1. Revenir à la migration précédente en base de données :
```bash
dotnet ef database update NomDeLaMigrationPrécédente -p Recrut.Data -s Recrut.Data
```
ou pour revenir avant toute migration :
```bash
dotnet ef database update 0 -p Recrut.Data -s Recrut.Data
```

2. Supprimer le fichier de migration :
```bash
dotnet ef migrations remove -p Recrut.Data -s Recrut.Data
```

Ces commandes restaureront votre base de données et supprimeront les fichiers de migration correspondants.