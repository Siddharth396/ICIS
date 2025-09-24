#!/bin/sh

 # Makes the script return immediately when a command fails rather than continue the execution
set -e

# Removes the directory if present.
rm -rf e2e_reports

# Creates a directory where the test report will be saved.
# It will be bound to a directory inside the Docker container (see docker-compose)"
mkdir e2e_reports

echo "Build docker image"

docker build -f ./DockerFiles/e2e-tests/e2e.Dockerfile -t "price-entry-e2e-tests:$CI_COMMIT_SHA" .
 
if docker run --name "price-entry-e2e-tests" -e ENV="${ENVIRONMENT}" -e CHAUSER="${CHAUSER}" -e CHAPASS="${CHAPASS}" price-entry-e2e-tests:$CI_COMMIT_SHA ; then
  echo "tests -- passed"
  docker cp "price-entry-e2e-tests":/app/playwright-report/ ./e2e_reports/
  # Clean up
  docker rm  price-entry-e2e-tests
  docker rmi price-entry-e2e-tests:$CI_COMMIT_SHA
  exit 0
else
  echo "tests -- failed"
  docker cp "price-entry-e2e-tests":/app/playwright-report/ ./e2e_reports/
  # Clean up
  docker rm  price-entry-e2e-tests
  docker rmi price-entry-e2e-tests:$CI_COMMIT_SHA
  exit 1
fi
