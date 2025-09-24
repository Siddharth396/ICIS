#!/bin/sh

##########################################################################
# Unit tests build & run script
#

# The idea is to build an image, then build a temporary container, run tests,
# and finally dispose of the container to keep Teamcity agents clean.
##########################################################################
# Makes the script return immediately when a command fails rather than continue the execution
set -e

# Proxy settings for Teamcity Agent usage
PROXY=$1

# Removes the directory if present.
rm -rf test_reports

# Creates a directory where the test report will be saved.
# It will be bound to a directory inside the Docker container (see docker-compose)"
mkdir test_reports

args="--build-arg http_proxy=${PROXY} --build-arg https_proxy=${PROXY} --build-arg SEM_VER=${semVer}"

docker build -f ./DockerFiles/unit-tests/tests.Dockerfile -t "price-entry-unit-tests:1.0.0" .

if docker run --name "price-entry-unit-tests" price-entry-unit-tests:1.0.0 ; then
  echo "tests -- passed"
  docker cp "price-entry-unit-tests":/app/coverage/ test_reports/
  # Clean up
  docker rm  price-entry-unit-tests
  docker rmi price-entry-unit-tests:1.0.0
  exit 0
else
  echo "tests -- failed"
  docker cp "price-entry-unit-tests":/app/coverage/ test_reports/
  # Clean up
  docker rm  price-entry-unit-tests
  docker rmi price-entry-unit-tests:1.0.0
  exit 1
fi
