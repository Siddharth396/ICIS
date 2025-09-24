#!/usr/bin/env bash

BUILD_COUNTER=${BUILD_COUNTER:-1}
docker_name="sbmfe/sbmfe-${PILET_NAME}"
docker_repo="artifacts.cha.rbxd.ds/${docker_name}"

docker_name_authoring="sbmfe/sbmfe-${PILET_NAME}-authoring"
docker_repo_authoring="artifacts.cha.rbxd.ds/${docker_name_authoring}"

docker pull gittools/gitversion:5.6.6

semver=$(docker run --rm -v /${PWD}:/repo -e BUILD_COUNTER gittools/gitversion:5.6.6 ../repo -output json -showvariable SemVer)
piletVersion="${semver}-${BUILD_COUNTER}"

echo "##teamcity[buildNumber '${piletVersion}']"

args="--build-arg no_proxy=${no_proxy} --build-arg http_proxy=${http_proxy} --build-arg https_proxy=${https_proxy} --build-arg SEM_VER=${piletVersion}  --build-arg FEED_SERVICE_URL=${FEED_SERVICE_URL}"

cp -r certificates ./packages/${PILET_NAME}/certificates

# copy builder customer facing pilet artifact for TC
mkdir -p  build/pilet
docker build ${args} -f ./packages/${PILET_NAME}/Dockerfile -t builder ./packages/${PILET_NAME} --target builder || exit -1
image_id=$(docker create builder)
docker cp "${image_id}":/app/pilet-package.tgz build/pilet/pilet-package.tgz
docker rm "${image_id}"

# create publishing image for customer facing pilet
docker build ${args} --pull -t "${docker_name}" -f ./packages/${PILET_NAME}/Dockerfile ./packages/${PILET_NAME} || exit -1

# push customer facing pilet docker image
docker tag "${docker_name}" "${docker_repo}:${piletVersion}"
docker push "${docker_repo}:${piletVersion}"

# copy builder authoring pilet artifact for TC
docker build ${args} --build-arg IS_AUTHORING=true -f ./packages/${PILET_NAME}/Dockerfile -t builder ./packages/${PILET_NAME} --target builder || exit -1
authoring_image_id=$(docker create builder)
docker cp "${authoring_image_id}":/app/pilet-package.tgz build/pilet/pilet-package-authoring.tgz
docker rm "${authoring_image_id}"

# create publishing image for authoring package
docker build ${args} --pull -t "${docker_name}" -f ./packages/${PILET_NAME}/Dockerfile ./packages/${PILET_NAME} || exit -1

# push authoring pilet docker image
docker tag "${docker_name_authoring}" "${docker_repo_authoring}:${piletVersion}"
docker push "${docker_repo_authoring}:${piletVersion}"

echo "##teamcity[setParameter name='PiletVersion' value='${piletVersion}']"

if [ "${IS_DEFAULT_BRANCH}" = "true" ]
then
  echo "##teamcity[setParameter name='dockerTagname' value='latest']"

  # customer facing
  docker tag  "${docker_name}" "${docker_repo}:latest"
  docker push "${docker_repo}:latest"

  #authoring
  docker tag "${docker_name_authoring}" "${docker_repo_authoring}:latest"
  docker push "${docker_repo_authoring}:latest"
fi

rm -rf ./packages/${PILET_NAME}/certificates
