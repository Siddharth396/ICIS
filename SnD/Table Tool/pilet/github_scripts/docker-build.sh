DOCKERUSERNAME=$1
DOCKERPASSWORD=$2

chmod +x ./github_scripts/docker_login.sh

./github_scripts/docker_login.sh $DOCKERUSERNAME $DOCKERPASSWORD

PILET_NAME="supply-and-demand-pilet"
docker_name="sbmfe-${PILET_NAME}"
docker_repo="${DOCKER_REGISTRY_CHA}/sbmfe/${docker_name}"
docker_repo_mockbff="${DOCKER_REGISTRY_CHA}/sbmfe/${docker_name}-mockbff"

docker_name_authoring="${docker_name}-authoring"
docker_repo_authoring="${DOCKER_REGISTRY_CHA}/sbmfe/${docker_name_authoring}"



echo "SEM_VERSION = ${SEM_VERSION}"
if [[ -z $SEM_VERSION ]]
then
    echo "Semantic version was not identified"
    exit 1;
fi

semVer=$SEM_VERSION
piletVersion=$SEM_VERSION

echo "sem: ${semVer}"

args="--build-arg SEM_VER=${semVer}"

# Pilet section
mkdir -p  build/pilet

# copy builder customer facing pilet artifact for TC
docker build ${args} -f ./packages/${PILET_NAME}/Dockerfile -t builder ./packages/${PILET_NAME} --target builder || exit -1

image_id=$(docker create builder)
docker cp "${image_id}":/app/pilet-package.tgz build/pilet/pilet-package.tgz

docker rm "${image_id}"

echo "Create publish customer facing image"
# create publishing image for customer facing pilet
docker build ${args} --pull -t "${docker_name}" -f ./packages/${PILET_NAME}/Dockerfile ./packages/${PILET_NAME}
docker tag "${docker_name}" "${docker_repo}:${piletVersion}"
docker push "${docker_repo}:${piletVersion}"

# copy builder authoring pilet artifact for TC
docker build ${args} --build-arg IS_AUTHORING=true -f ./packages/${PILET_NAME}/Dockerfile -t builder ./packages/${PILET_NAME} --target builder || exit -1

authoring_image_id=$(docker create builder)
docker cp "${authoring_image_id}":/app/pilet-package.tgz build/pilet/pilet-package-authoring.tgz
docker rm "${authoring_image_id}"

echo "Create publish authoring image"
# create publishing image for authoring package
docker build ${args} --build-arg IS_AUTHORING=true --pull -t "${docker_name_authoring}" -f ./packages/${PILET_NAME}/Dockerfile ./packages/${PILET_NAME} || exit -1
docker tag "${docker_name_authoring}" "${docker_repo_authoring}:${piletVersion}"
docker push "${docker_repo_authoring}:${piletVersion}"

if [ $CI_COMMIT_BRANCH == $CI_DEFAULT_BRANCH ]; then
  #customer facing
  docker tag  "${docker_name}" "${docker_repo}:latest"
  docker push "${docker_repo}:latest"

  #authoring
  docker tag "${docker_name_authoring}" "${docker_repo_authoring}:latest"
  docker push "${docker_repo_authoring}:latest"
fi

if [[ "$GIT_BRANCH" == *"release/"* ]]; then
  echo "Sending release tag to docker_repo"

  #customer facing
  docker tag "${docker_name}" "${docker_repo}:prod-1-${piletVersion}"
  docker push "${docker_repo}:prod-1-${piletVersion}"

  #authoring
  docker tag "${docker_name_authoring}" "${docker_repo_authoring}:prod-1-${piletVersion}"
  docker push "${docker_repo_authoring}:prod-1-${piletVersion}"
fi

#  undo comment here
echo "Create mock bff image"
mockBffName="${docker_name}-mockbff"
docker build ${args} --pull -t "${mockBffName}" -f ./packages/bff-mock/Dockerfile ./packages/bff-mock

docker tag "${mockBffName}" "${docker_repo_mockbff}:${piletVersion}"
docker push "${docker_repo_mockbff}:${piletVersion}"

if [ $CI_COMMIT_BRANCH == $CI_DEFAULT_BRANCH ]; then
  docker tag  "${mockBffName}" "${docker_repo_mockbff}:latest"
  docker push "${docker_repo_mockbff}:latest"
fi
# End Mock bff section


# output the docker tag for next stage
echo "BUILD_TAG=$piletVersion" >> artifact.env