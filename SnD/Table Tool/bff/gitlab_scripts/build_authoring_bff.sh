# exit on any error
set -e

if [[ -z $SEM_VERSION ]]
then
    echo "Semantic version was not identified"
    exit 1;
fi

dockerRepo="artifacts.cha.rbxd.ds"

# update docker name as per project asset id defined by SRE
# dockerRepo=repo/assetid/assetid-containername:tagname
dockerImageName="snd/sndctl/authoring-bff"

tagname="${SEM_VERSION}"

echo "*************************************************************"
echo "************* Tagname is  ${tagname}"
echo "*************************************************************"

#pull the docker images to ensure they are the correct supported version
docker pull mcr.microsoft.com/dotnet/sdk:8.0-alpine
docker pull mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# perform the first stage build and extract the coverage report

echo "*************************************************************"
echo "******* Build final Docker image and publish to nexus *******"
echo "*************************************************************"

buildTag="${dockerImageName}:build-${tagname}"

args=""

docker build -f src/main/docker/Authoring/Dockerfile -t ${buildTag} ${args} --build-arg FULL_SEM_VERSION=${tagname} .

docker save ${buildTag} > authoringBffBuild.tar
