# Guide d'Exécution des Tests Unitaires

## Exécution des Tests dans Visual Studio

1. Ouvrez le projet dans Visual Studio
2. Ouvrez l'Explorateur de Tests (Test > Windows > Explorateur de Tests)
3. Cliquez sur "Exécuter tous les tests" ou sélectionnez des tests spécifiques

## Exécution des Tests en Ligne de Commande

```bash
# À la racine du projet
dotnet test Recrut.Tests/Recrut.Tests.csproj

# Avec génération de rapport de couverture
dotnet test Recrut.Tests/Recrut.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/Coverage/

# Avec le fichier .runsettings
dotnet test Recrut.Tests/Recrut.Tests.csproj --settings .\.runsettings
```

## Génération d'un Rapport HTML de Couverture

Pour générer un rapport HTML à partir du fichier de couverture Cobertura, vous pouvez utiliser un outil comme ReportGenerator :

```bash
# Installation de l'outil ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Génération du rapport HTML
reportgenerator "-reports:Recrut.Tests/TestResults/Coverage/coverage.cobertura.xml" "-targetdir:Recrut.Tests/TestResults/Coverage/Reports" "-reporttypes:Html"
```

Le rapport HTML sera disponible dans le dossier `Recrut.Tests/TestResults/Coverage/Reports`.




# Bonnes Pratiques pour les Tests Unitaires

## Principes Fondamentaux

### 1. Isolation des Tests
- Chaque test doit être indépendant des autres
- Utilisez des mocks pour isoler le comportement des dépendances
- Assurez-vous que l'état de l'application n'est pas partagé entre les tests

### 2. Convention de Nommage Claire
- Utilisez une convention cohérente comme `[Méthode]_[Condition]_[RésultatAttendu]`
- Exemple : `AuthenticateAsync_WithInvalidEmail_ShouldReturnFailure`

### 3. Structure AAA (Arrange-Act-Assert)
- **Arrange** : Préparez les données et conditions préalables
- **Act** : Exécutez l'action à tester
- **Assert** : Vérifiez les résultats

## Bonnes Pratiques Spécifiques à .NET

### 1. Utilisez xUnit Efficacement
- Utilisez `[Theory]` et `[InlineData]` pour les tests paramétrés
- Utilisez les fixtures de test pour le partage de code d'initialisation
- Exploitez les collections de tests pour les dépendances partagées

### 2. Mocking avec Moq
- Limitez les comportements simulés au strict nécessaire
- Utilisez `Verify()` pour confirmer que certaines méthodes ont été appelées
- Configurez les comportements avec des paramètres `It.Is<T>` pour les scénarios complexes

### 3. Tests Asynchrones
- Utilisez `async`/`await` plutôt que `.Result` ou `.Wait()`
- Retournez `Task` depuis les méthodes de test asynchrones
- Utilisez `Task.FromResult` pour simuler des opérations asynchrones dans les mocks

## Stratégies pour les Services d'Authentification

### 1. Tests de JWT
- Validez la structure du token (format JWT)
- Vérifiez les claims essentiels (subject, issuer, audience)
- Testez l'expiration des tokens

### 2. Tests de Hachage de Mot de Passe
- Testez que le même mot de passe donne des hachages différents (salt unique)
- Vérifiez que différents mots de passe donnent des hachages différents
- Testez la validation correcte d'un mot de passe contre son hachage
- Testez les cas limites (mot de passe vide, formats de hachage invalides)

## Pratiques Avancées

### 1. Couverture de Code
- Visez une couverture minimale de 80% pour les services critiques
- Ne poursuivez pas la couverture à 100% au détriment de la qualité des tests
- Concentrez-vous sur la couverture des chemins de code critiques

### 2. Tests de Limites et Cas d'Erreur
- Testez les valeurs limites (chaînes vides, valeurs nulles, etc.)
- Vérifiez que les exceptions appropriées sont levées
- Testez les scénarios d'erreur spécifiques à votre domaine

### 3. Évitez les Anti-Patterns
- Évitez les tests qui dépendent les uns des autres
- Évitez les assertions multiples non liées dans un même test
- Évitez de tester le framework lui-même