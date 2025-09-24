set -e

# run whitesource and extract report
docker rmi --force wsbuilder || true

DOCKER_IMAGE="prcent/mongodb-seeder"
DOCKER_TAGNAME="build-${SEM_VERSION}"
CODE_FOLDER_PATH="/app"
WS_INCLUDES_PATTERN="**/*.exe **/*.dll"
WS_PROJECT_NAME="price-entry - MongoDb Seeder"

echo "Running WhiteSource scan from ${DOCKER_IMAGE}:${DOCKER_TAGNAME} -> ${CODE_FOLDER_PATH}"

docker load --input mongoDbSeederBuild.tar

args="--build-arg WS_API_KEY=${WHITESOURCE_APIKEY} --build-arg DOCKER_TAGNAME=${DOCKER_TAGNAME} --build-arg DOCKER_IMAGE=${DOCKER_IMAGE} --build-arg CODE_FOLDER_PATH=${CODE_FOLDER_PATH}"
docker build -f ./build_tools/whitesource/Dockerfile -t wsbuilder ${args} --build-arg WS_PROJECT_NAME="${WS_PROJECT_NAME}" --build-arg WS_INCLUDES_PATTERN="${WS_INCLUDES_PATTERN}" ./build_tools/whitesource
image_id=$(docker create wsbuilder)
docker cp "${image_id}":/build/whitesource/. whitesource
docker rm "${image_id}"
docker rmi --force wsbuilder
