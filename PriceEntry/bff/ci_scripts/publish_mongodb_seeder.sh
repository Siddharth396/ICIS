# exit on any error
set -e

if [[ -z $SEM_VERSION ]]
then
    echo "Semantic version was not identified"
    exit 1;
fi

dockerRegistry="${DOCKER_REGISTRY_CHA}"

# update docker name as per project asset id defined by SRE
# dockerRepo=repo/assetid/assetid-containername:tagname
dockerImageName="prcent/mongodb-seeder"

tagname="${SEM_VERSION}"

buildTag="${dockerImageName}:build-${tagname}"

echo "*************************************************************"
echo "******* Publish to nexus ${dockerRegistry}/${dockerImageName}:${tagname}"
echo "*************************************************************"

docker load --input mongoDbSeederBuild.tar
docker tag ${buildTag} "${dockerRegistry}/${dockerImageName}:${tagname}"
docker push "${dockerRegistry}/${dockerImageName}:${tagname}"

# push 'latest' tag for default branch
if [ $CI_COMMIT_BRANCH == $CI_DEFAULT_BRANCH ]
then

  echo "*************************************************************"
  echo "******* Publish to nexus ${dockerRegistry}/${dockerImageName}:latest"
  echo "*************************************************************"
    
  docker tag ${buildTag} "${dockerRegistry}/${dockerImageName}:latest"
  docker push "${dockerRegistry}/${dockerImageName}:latest"
fi
