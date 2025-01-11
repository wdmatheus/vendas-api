#!/usr/bin/env bash

CURRENT_DIR="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

rm -rf "$CURRENT_DIR"/TestResults;

dotnet tool update -g dotnet-reportgenerator-globaltool;

dotnet clean;

dotnet test --collect:"XPlat Code Coverage" --settings "$CURRENT_DIR"/coverlet.settings.xml;

#path=$(ls -d -1 ./TestResults/* | sed -n '1p');
#echo "$path"

reportgenerator \
 -reports:"$CURRENT_DIR/TestResults/**/coverage.cobertura.xml" \
 -targetdir:"$CURRENT_DIR/TestResults/report" \
 -riskhotspotassemblyfilters:"-TesteOminaGht.Core" \
 -reporttypes:Html;

echo -e "\nOpen report results on file bellow:\n"
echo -e "file://${CURRENT_DIR}/TestResults/report/index.html"
echo -e "\n"

xdg-open || open "$CURRENT_DIR"/TestResults/report/index.html;
