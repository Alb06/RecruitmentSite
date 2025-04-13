# Tests d'intégration de l'API Recrut

Ce projet contient les tests d'intégration pour l'API Recrut, conçus pour vérifier le bon fonctionnement des endpoints API et leurs interactions avec les différentes couches applicatives.

## Objectifs des tests d'intégration

Contrairement aux tests unitaires qui isolent et testent des composants individuels, les tests d'intégration vérifient que:
- Les différentes parties de l'application fonctionnent correctement ensemble
- Les requêtes HTTP sont correctement routées vers les contrôleurs appropriés
- Les réponses HTTP sont conformes aux attentes (codes d'état, format, contenu)
- Les interactions avec la base de données sont correctes
- Les middlewares et filtres fonctionnent comme prévu (authentification, gestion des exceptions, etc.)

## Architecture des tests

L'architecture de tests d'intégration repose sur les composants suivants:

1. **CustomWebApplicationFactory**: Crée une instance de l'application web pour les tests, en remplaçant certains services (comme la base de données) par des alternatives adaptées aux tests.

2. **TestDataSeeder**: Initialise la base de données en mémoire avec des données de test prédéfinies.

3. **Tests des contrôleurs**: Vérifient les fonctionnalités spécifiques de chaque contrôleur.

## Frameworks et outils utilisés

- **xUnit**: Framework de test
- **Microsoft.AspNetCore.Mvc.Testing**: Facilite les tests d'intégration des applications ASP.NET Core
- **EntityFrameworkCore.InMemory**: Fournit une base de données en mémoire pour les tests
- **FluentAssertions** (optionnel): Améliore la lisibilité des assertions

## Exécuter les tests d'intégration

### Via Visual Studio

1. Ouvrez la solution dans Visual Studio
2. Ouvrez l'Explorateur de tests (Test > Explorateur de tests)
3. Cliquez sur "Exécuter tout" ou sélectionnez des tests spécifiques à exécuter

### Via la ligne de commande

```bash
# Exécuter tous les tests
dotnet test Recrut.API.IntegrationTests/Recrut.API.IntegrationTests.csproj

# Exécuter avec rapport de couverture
dotnet test Recrut.API.IntegrationTests/Recrut.API.IntegrationTests.csproj --collect:"XPlat Code Coverage" --settings .runsettings

# Exécuter un test spécifique
dotnet test Recrut.API.IntegrationTests/Recrut.API.IntegrationTests.csproj --filter "FullyQualifiedName=Recrut.API.IntegrationTests.Controllers.AuthControllerTests.Login_WithValidCredentials_ReturnsToken"
```

## Structure du projet

```
Recrut.API.IntegrationTests/
├── CustomWebApplicationFactory.cs  // Configuration de l'environnement de test
├── TestDataSeeder.cs               // Initialisation des données de test
├── HttpClientExtensions.cs         // Extensions utilitaires pour HttpClient
├── Controllers/                    // Tests groupés par contrôleur
│   ├── AuthControllerTests.cs      // Tests du contrôleur d'authentification
│   └── UsersControllerTests.cs     // Tests du contrôleur utilisateurs
└── Recrut.API.IntegrationTests.csproj
```

## Base de données de test

Par défaut, ces tests utilisent une base de données en mémoire (`UseInMemoryDatabase`) qui est réinitialisée pour chaque exécution de test. Cette approche offre plusieurs avantages:

- Isolation complète entre les tests
- Exécution rapide sans dépendance externe
- Pas besoin de nettoyer les données après les tests

Cependant, la base de données en mémoire ne prend pas en charge toutes les fonctionnalités d'une base de données relationnelle. Pour des tests plus fidèles, vous pouvez également configurer le projet pour utiliser SQLite en mémoire, qui implémente davantage de fonctionnalités relationnelles.

## Authentification dans les tests

Certains endpoints nécessitent une authentification. Pour tester ces endpoints, nous:
1. Obtenons d'abord un token JWT valide via l'endpoint `/api/auth/login`
2. Incluons ce token dans les en-têtes des requêtes suivantes

Cette approche teste l'intégration complète du système d'authentification.

## Bonnes pratiques pour ajouter de nouveaux tests

1. **Nommage des méthodes de test**: Utilisez le format `[Méthode]_[Scénario]_[RésultatAttendu]` 
   Ex: `Login_WithInvalidPassword_ReturnsUnauthorized`

2. **Indépendance des tests**: Chaque test doit être indépendant et ne pas dépendre d'autres tests

3. **Assertions complètes**: Vérifiez non seulement les codes d'état HTTP, mais aussi le contenu des réponses

4. **Tests des cas limites**: Incluez des tests pour les cas d'erreur et les cas limites, pas seulement les cas nominaux

5. **Organisation des tests**: Gardez les tests organisés par fonctionnalité ou par contrôleur

## Différences avec les tests unitaires

| Tests Unitaires | Tests d'Intégration |
|-----------------|---------------------|
| Testent des composants isolés | Testent des interactions entre composants |
| Utilisent des mocks/stubs | Utilisent des implémentations réelles ou proches |
| Rapides à exécuter | Peuvent être plus lents à exécuter |
| Aident à localiser précisément les bugs | Valident le comportement global du système |
| Préférez-les pour la logique métier complexe | Préférez-les pour les flux de données et les API |

Les deux types de tests sont complémentaires et devraient être utilisés ensemble pour une couverture optimale.

## Conseils pour le débogage

Si un test échoue, vous pouvez:

1. Utiliser les journaux de debug pour identifier le problème
2. Ajouter des points d'arrêt dans le code des tests et exécuter en mode debug
3. Examiner la base de données de test à l'aide de:
   ```csharp
   var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
   // Inspecter db.Users, etc.
   ```

## Rapport de couverture

Pour générer un rapport de couverture de code:

1. Exécutez les tests avec l'option de couverture
2. Utilisez ReportGenerator pour produire un rapport HTML:

```bash
# Installer ReportGenerator (si nécessaire)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Générer le rapport
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:Html

# Ouvrir le rapport
start ./TestResults/CoverageReport/index.html
```

## Intégration CI/CD

Ces tests d'intégration peuvent être exécutés dans votre pipeline GitLab CI en ajoutant une étape dédiée dans votre fichier `.gitlab-ci.yml`:

```yaml
integration_tests:
  stage: test
  script:
    - dotnet test Recrut.API.IntegrationTests/Recrut.API.IntegrationTests.csproj --collect:"XPlat Code Coverage"
  artifacts:
    paths:
      - TestResults/
```