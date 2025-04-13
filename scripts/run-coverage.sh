#!/bin/bash
# Script d'exécution des tests avec mesure de couverture

# Obtenir le chemin du répertoire contenant ce script
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
# Obtenir le chemin de la racine de la solution (dossier parent du répertoire scripts)
SOLUTION_DIR="$( dirname "$SCRIPT_DIR" )"

# Se déplacer à la racine de la solution
cd "$SOLUTION_DIR"

# Nettoyer les résultats précédents
echo "Nettoyage des anciens résultats de test..."
rm -rf ./TestResults

# Exécuter les tests avec la couverture
echo "Exécution des tests avec analyse de couverture..."
dotnet test Recrut.Tests/Recrut.Tests.csproj \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --settings .runsettings \
  --results-directory ./TestResults

# Vérifier si l'exécution des tests a réussi
if [ $? -ne 0 ]; then
  echo "❌ L'exécution des tests a échoué"
  exit 1
fi

# Vérifier si ReportGenerator est installé
if ! command -v reportgenerator &> /dev/null; then
  echo "Installation de l'outil ReportGenerator..."
  dotnet tool install -g dotnet-reportgenerator-globaltool
fi

# Générer un rapport HTML
echo "Génération du rapport de couverture..."
reportgenerator \
  -reports:"./TestResults/**/coverage.cobertura.xml" \
  -targetdir:"./TestResults/CoverageReport" \
  -reporttypes:Html

echo "✅ Rapport de couverture généré avec succès"
echo "📊 Consultez le rapport dans ./TestResults/CoverageReport/index.html"

# Ouvrir le rapport selon le système d'exploitation
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
  # Windows
  start ./TestResults/CoverageReport/index.html
elif [[ "$OSTYPE" == "darwin"* ]]; then
  # macOS
  open ./TestResults/CoverageReport/index.html
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
  # Linux
  if command -v xdg-open &> /dev/null; then
    xdg-open ./TestResults/CoverageReport/index.html
  else
    echo "Pour ouvrir le rapport manuellement : $SOLUTION_DIR/TestResults/CoverageReport/index.html"
  fi
else
  echo "Pour ouvrir le rapport manuellement : $SOLUTION_DIR/TestResults/CoverageReport/index.html"
fi