# Ajout de fonctionnalit�s d'audit aux entit�s

## Comment impl�menter l'interface IAuditable dans une nouvelle entit�

Pour ajouter les fonctionnalit�s d'audit (suivi des dates de cr�ation et de modification) � une nouvelle entit�, suivez ces �tapes :

1. Assurez-vous que votre classe impl�mente l'interface `IAuditable` :

```csharp
using Recrut.Models.Interfaces;
using System;

namespace Recrut.Models
{
    public class NouvelleEntite : IEntity, IAuditable
    {
        // Propri�t�s de l'entit�...
        public int Id { get; set; }
        public required string Attribut1 { get; set; }
        public required string Attribut2 { get; set; }
        
        // Propri�t�s d'audit requises par l'interface IAuditable
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
```

2. Ajoutez la configuration dans la m�thode `OnModelCreating` de `AppDbContext` (facultatif, mais recommand�) :

```csharp
// Dans la m�thode OnModelCreating de AppDbContext
modelBuilder.Entity<NouvelleEntite>()
    .Property(e => e.CreatedAt)
    .HasColumnType("timestamp with time zone")
    .IsRequired();
    
modelBuilder.Entity<NouvelleEntite>()
    .Property(e => e.UpdatedAt)
    .HasColumnType("timestamp with time zone")
    .IsRequired(false);
```

3. Cr�ez une migration pour ajouter la nouvelle entit� � la base de donn�es :

```bash
dotnet ef migrations add AddNouvelleEntite -p Recrut.Data -s Recrut.Data
dotnet ef database update -p Recrut.Data -s Recrut.Data
```

## Comportement attendu

- Lorsqu'une entit� est cr��e, la propri�t� `CreatedAt` est automatiquement initialis�e avec la date et l'heure actuelles en UTC
- Lorsqu'une entit� est modifi�e via la m�thode `UpdateAsync` du repository, la propri�t� `UpdatedAt` est automatiquement mise � jour avec la date et l'heure actuelles en UTC
- Ces informations sont persist�es dans la base de donn�es et peuvent �tre utilis�es pour l'audit et le suivi des modifications

# Suppression de migrations Entity Framework Core

## Migration cr��e mais pas appliqu�e
Pour supprimer un fichier de migration non appliqu� :
```bash
dotnet ef migrations remove -p Recrut.Data -s Recrut.Data
```

## Migration d�j� appliqu�e en base
Deux �tapes sont n�cessaires :

1. Revenir � la migration pr�c�dente en base de donn�es :
```bash
dotnet ef database update NomDeLaMigrationPr�c�dente -p Recrut.Data -s Recrut.Data
```
ou pour revenir avant toute migration :
```bash
dotnet ef database update 0 -p Recrut.Data -s Recrut.Data
```

2. Supprimer le fichier de migration :
```bash
dotnet ef migrations remove -p Recrut.Data -s Recrut.Data
```

Ces commandes restaureront votre base de donn�es et supprimeront les fichiers de migration correspondants.