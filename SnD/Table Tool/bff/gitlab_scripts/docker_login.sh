# exit on any error
set -e

echo "${DOCKER_PASSWORD}" | docker login --username "${DOCKER_USERNAME}" --password-stdin "${DOCKER_REGISTRY_CHA}"