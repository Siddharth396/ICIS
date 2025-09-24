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
dockerImageName="prcent/mongodb-seeder"

tagname="${SEM_VERSION}"

echo "*************************************************************"
echo "************* Tagname is  ${tagname}"
echo "*************************************************************"

# perform the first stage build and extract the coverage report

echo "*************************************************************"
echo "******* Build final Docker image and publish to nexus *******"
echo "*************************************************************"

buildTag="${dockerImageName}:build-${tagname}"

args=""

docker build -f src/main/docker/MongoDbSeeder/Dockerfile -t ${buildTag} ${args} --build-arg FULL_SEM_VERSION=${tagname} .

docker save ${buildTag} > mongoDbSeederBuild.tar
