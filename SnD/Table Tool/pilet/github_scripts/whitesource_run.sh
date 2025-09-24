
# Makes the script return immediately when a command fails rather than continue the execution
set -e
APIKEY=$1
USERKEY=$2


# Removes the directory if present.
rm -rf whitesource

# Creates a directory where the test report will be saved.
# It will be bound to a directory inside the Docker container (see docker-compose)"
mkdir whitesource

docker build -f ./DockerFiles/whitesource/Dockerfile -t "supply-and-demand-white-source:1.0.0" ./DockerFiles/whitesource
volumeArgs="--mount src=$(pwd)/packages/supply-and-demand-pilet,target=/source,type=bind"
whiteSourceArgs="-e WS_PROJECTNAME=supply-and-demand-mfe -e APIKEY=$APIKEY -e USERKEY=$USERKEY"

if docker run ${volumeArgs} ${whiteSourceArgs} --name supply-and-demand-white-source supply-and-demand-white-source:1.0.0 ; then
  echo "tests -- passed"
  docker cp "supply-and-demand-white-source":/whitesource/ whitesource/
  # Clean up
  docker rm  supply-and-demand-white-source
  docker rmi supply-and-demand-white-source:1.0.0
  exit 0
else
  echo "tests -- failed"
  docker cp "supply-and-demand-white-source":/whitesource/ whitesource/
  # Clean up
  docker rm  supply-and-demand-white-source
  docker rmi supply-and-demand-white-source:1.0.0
  exit 1
fi

