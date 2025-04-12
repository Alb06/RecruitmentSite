# Guide d'Ex�cution des Tests Unitaires

## Ex�cution des Tests dans Visual Studio

1. Ouvrez le projet dans Visual Studio
2. Ouvrez l'Explorateur de Tests (Test > Windows > Explorateur de Tests)
3. Cliquez sur "Ex�cuter tous les tests" ou s�lectionnez des tests sp�cifiques

## Ex�cution des Tests en Ligne de Commande

```bash
# � la racine du projet
dotnet test Recrut.Tests/Recrut.Tests.csproj

# Avec g�n�ration de rapport de couverture
dotnet test Recrut.Tests/Recrut.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/Coverage/

# Avec le fichier .runsettings
dotnet test Recrut.Tests/Recrut.Tests.csproj --settings .\.runsettings
```

## G�n�ration d'un Rapport HTML de Couverture

Pour g�n�rer un rapport HTML � partir du fichier de couverture Cobertura, vous pouvez utiliser un outil comme ReportGenerator :

```bash
# Installation de l'outil ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# G�n�ration du rapport HTML
reportgenerator "-reports:Recrut.Tests/TestResults/Coverage/coverage.cobertura.xml" "-targetdir:Recrut.Tests/TestResults/Coverage/Reports" "-reporttypes:Html"
```

Le rapport HTML sera disponible dans le dossier `Recrut.Tests/TestResults/Coverage/Reports`.




# Bonnes Pratiques pour les Tests Unitaires

## Principes Fondamentaux

### 1. Isolation des Tests
- Chaque test doit �tre ind�pendant des autres
- Utilisez des mocks pour isoler le comportement des d�pendances
- Assurez-vous que l'�tat de l'application n'est pas partag� entre les tests

### 2. Convention de Nommage Claire
- Utilisez une convention coh�rente comme `[M�thode]_[Condition]_[R�sultatAttendu]`
- Exemple : `AuthenticateAsync_WithInvalidEmail_ShouldReturnFailure`

### 3. Structure AAA (Arrange-Act-Assert)
- **Arrange** : Pr�parez les donn�es et conditions pr�alables
- **Act** : Ex�cutez l'action � tester
- **Assert** : V�rifiez les r�sultats

## Bonnes Pratiques Sp�cifiques � .NET

### 1. Utilisez xUnit Efficacement
- Utilisez `[Theory]` et `[InlineData]` pour les tests param�tr�s
- Utilisez les fixtures de test pour le partage de code d'initialisation
- Exploitez les collections de tests pour les d�pendances partag�es

### 2. Mocking avec Moq
- Limitez les comportements simul�s au strict n�cessaire
- Utilisez `Verify()` pour confirmer que certaines m�thodes ont �t� appel�es
- Configurez les comportements avec des param�tres `It.Is<T>` pour les sc�narios complexes

### 3. Tests Asynchrones
- Utilisez `async`/`await` plut�t que `.Result` ou `.Wait()`
- Retournez `Task` depuis les m�thodes de test asynchrones
- Utilisez `Task.FromResult` pour simuler des op�rations asynchrones dans les mocks

## Strat�gies pour les Services d'Authentification

### 1. Tests de JWT
- Validez la structure du token (format JWT)
- V�rifiez les claims essentiels (subject, issuer, audience)
- Testez l'expiration des tokens

### 2. Tests de Hachage de Mot de Passe
- Testez que le m�me mot de passe donne des hachages diff�rents (salt unique)
- V�rifiez que diff�rents mots de passe donnent des hachages diff�rents
- Testez la validation correcte d'un mot de passe contre son hachage
- Testez les cas limites (mot de passe vide, formats de hachage invalides)

## Pratiques Avanc�es

### 1. Couverture de Code
- Visez une couverture minimale de 80% pour les services critiques
- Ne poursuivez pas la couverture � 100% au d�triment de la qualit� des tests
- Concentrez-vous sur la couverture des chemins de code critiques

### 2. Tests de Limites et Cas d'Erreur
- Testez les valeurs limites (cha�nes vides, valeurs nulles, etc.)
- V�rifiez que les exceptions appropri�es sont lev�es
- Testez les sc�narios d'erreur sp�cifiques � votre domaine

### 3. �vitez les Anti-Patterns
- �vitez les tests qui d�pendent les uns des autres
- �vitez les assertions multiples non li�es dans un m�me test
- �vitez de tester le framework lui-m�me