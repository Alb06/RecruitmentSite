# Script d'exécution des tests avec mesure de couverture (PowerShell)
# Placez ce fichier dans le dossier 'scripts' à la racine de la solution

# Obtenir le chemin du répertoire contenant ce script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
# Obtenir le chemin de la racine de la solution (dossier parent du répertoire scripts)
$solutionDir = Split-Path -Parent $scriptDir

# Se déplacer à la racine de la solution
Set-Location $solutionDir

# Nettoyer les résultats précédents
Write-Host "Nettoyage des anciens résultats de test..." -ForegroundColor Cyan
if (Test-Path "./TestResults") {
    Remove-Item -Path "./TestResults" -Recurse -Force
}

# Exécuter les tests avec la couverture
Write-Host "Exécution des tests avec analyse de couverture..." -ForegroundColor Cyan
dotnet test Recrut.Tests/Recrut.Tests.csproj `
    --configuration Release `
    --collect:"XPlat Code Coverage" `
    --settings .runsettings `
    --results-directory ./TestResults

# Vérifier si l'exécution des tests a réussi
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ L'exécution des tests a échoué" -ForegroundColor Red
    exit 1
}

# Vérifier si ReportGenerator est installé
$reportGeneratorInstalled = $null
try {
    $reportGeneratorInstalled = Get-Command reportgenerator -ErrorAction SilentlyContinue
} catch {
    $reportGeneratorInstalled = $null
}

if ($null -eq $reportGeneratorInstalled) {
    Write-Host "Installation de l'outil ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Générer un rapport HTML
Write-Host "Génération du rapport de couverture..." -ForegroundColor Cyan
reportgenerator `
    -reports:"./TestResults/**/coverage.cobertura.xml" `
    -targetdir:"./TestResults/CoverageReport" `
    -reporttypes:Html

Write-Host "✅ Rapport de couverture généré avec succès" -ForegroundColor Green
Write-Host "📊 Consultez le rapport dans ./TestResults/CoverageReport/index.html" -ForegroundColor White

# Ouvrir le rapport
$reportPath = Join-Path -Path $solutionDir -ChildPath "TestResults\CoverageReport\index.html"
if (Test-Path $reportPath) {
    Write-Host "Ouverture du rapport..." -ForegroundColor Cyan
    Invoke-Item $reportPath
} else {
    Write-Host "Le rapport n'a pas été trouvé à l'emplacement attendu: $reportPath" -ForegroundColor Yellow
}
