
# Makes the script return immediately when a command fails rather than continue the execution
APIKEY=$1
USERKEY=$2

# Removes the directory if present.
rm -rf whitesource

# Creates a directory where the test report will be saved.
# It will be bound to a directory inside the Docker container (see docker-compose)"
mkdir whitesource

image_name="price-entry-white-source"
image_tag="1.0.0"

docker stop $image_name || true && docker rm $image_name || true
docker image rm $image_name:$image_tag || true

volume_args="--mount src="$(pwd)",target=/source,type=bind"
ws_args="-e WS_PROJECTNAME=priceentry-mfe -e APIKEY=$APIKEY -e USERKEY=$USERKEY"

docker build -f ./DockerFiles/whitesource/Dockerfile -t $image_name:$image_tag ${build_args} ./DockerFiles/whitesource

if docker run ${volume_args} ${ws_args} --name $image_name $image_name:$image_tag ; then
  echo "All dependencies conform with open source policies"
  docker cp $image_name:/whitesource/ whitesource/
  docker rm $image_name
  docker rmi $image_name:$image_tag
  exit 0
else
  echo "Not all dependencies conform with open source policies"
  docker cp $image_name:/whitesource/ whitesource/
  docker rm $image_name
  docker rmi $image_name:$image_tag
  exit 1
fi