#!/bin/bash
# Exit on any error
set -e

echo "*************************************************************"
echo "********* k6 tests build & run script ********"
echo "*************************************************************"

# Removes the directory if present.
rm -rf k6-report

# Creates a directory where the test report will be saved.
mkdir k6-report

# Build Docker Compose services
echo "Building Docker Compose services..."
docker-compose -f "./src/test/non-functional/priceEntry/docker-compose.yml" build

# Run Docker Compose and handle success/failure
echo "Starting Docker Compose services..."

docker-compose -f "./src/test/non-functional/priceEntry/docker-compose.yml" up

# Wait for a moment to ensure the container starts
sleep 5

# Capture the exit code of the k6 tests container
EXIT_CODE=$(docker inspect -f '{{.State.ExitCode}}' priceEntry-k6-api-tests)

if [ "$EXIT_CODE" -ne 0 ]; then
  echo "Tests -- failed with exit code $EXIT_CODE"
  echo "tests -- failed"

  docker cp "priceEntry-k6-api-tests":/app/html-results/ ./html-results
  docker rm priceEntry-k6-api-tests
  exit $EXIT_CODE # Use the captured exit code
else
  echo "tests -- passed"
  docker cp "priceEntry-k6-api-tests":/app/html-results/ ./html-results
  docker rm priceEntry-k6-api-tests
  exit 0
fi
