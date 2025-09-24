# exit on any error
set -e

echo "*************************************************************"
echo "********* Run unit tests and extract coverage report ********"
echo "*************************************************************"

mkdir -p  build/coverage
docker build -f src/main/docker/Test/Dockerfile -t builder . --target coverage

image_id=$(docker create builder)
docker cp "${image_id}":/app/coverage/. build/coverage
docker cp "${image_id}":/artifacts/. ./artifacts
docker rm "${image_id}"

# check code coverage and fail pipeline if coverage is less than 100%
echo "Checking code coverage..."
targetCoverage="$target_coverage"
coverage_file="./build/coverage/report/Cobertura.xml"

if [ ! -f "$coverage_file" ]; then
  echo -e "\e[31mError: Code coverage file not found: $coverage_file\e[0m"
  exit 1
fi

coverage=$(sed -n 's/.*line-rate="\([^"]*\)".*/\1/p' "$coverage_file" | head -n 1)

if [ -z "$coverage" ]; then
  echo -e "\e[31mError: Coverage value not found in $coverage_file\e[0m"
  exit 1
fi

coverage=$(echo "$coverage * 100" | bc -l)

isCoverageLessThan=$(echo "$coverage < $targetCoverage" |bc -l)

if [[ $isCoverageLessThan -eq "1" ]]; then
  echo -e "\e[31mCode coverage is not $targetCoverage% ($coverage%). Failing pipeline.\e[0m"
  exit 1
fi

echo "Code coverage is ($coverage%)"