#!/bin/bash
set -e

SEM_VERSION=$1

if [[ -z $SEM_VERSION ]]
then
    echo "Semantic version was not identified"
    exit 1;
fi


tagname="${SEM_VERSION}"
ImageName="snd-charts"
BaseRepo="supdem"
ImageRepo="artifacts.cha.rbxd.ds/${BaseRepo}/${ImageName}:${tagname}"

echo "*************************************************************"
echo "************* Helm Chart Tagname is ${tagname}"
echo "*************************************************************"


# Build Image and publish to Nexus


echo "*************************************************************"
echo "******* Build Helm Chart Docker image"
echo "*************************************************************"


buildTag="${ImageName}:${tagname}" 

args=""

docker build -t ${buildTag} ${args} -f Helm.Dockerfile .

echo "*************************************************************"
echo "******* Publish to nexus ${ImageRepo}"
echo "*************************************************************"

docker tag "${buildTag}" "${ImageRepo}"
docker push "${ImageRepo}"


#cleanup

docker rmi "${buildTag}"
