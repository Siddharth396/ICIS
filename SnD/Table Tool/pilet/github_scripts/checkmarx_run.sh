set -e
# rm ./DockerFiles/checkmarx/source -rf

# mkdir ./DockerFiles/checkmarx/source
# cp ./packages/snd-table-pilet/ ./DockerFiles/checkmarx/source -r

cd ./DockerFiles/checkmarx
buildArgs="--build-arg CHECKMARX_USER=$CHECKMARX_USER --build-arg CHECKMARX_PROJECT_NAME=$CHECKMARX_PROJECT_NAME --build-arg CHECKMARX_PASS=$CHECKMARX_PASS"
args="-e CHECKMARX_USER=$CHECKMARX_USER -e CHECKMARX_PROJECT_NAME=$CHECKMARX_PROJECT_NAME -e CHECKMARX_PASS=$CHECKMARX_PASS"
echo "starting"
docker build ${buildArgs} -t checkmarx -f deploy.Dockerfile .

echo "built"
volMount="-v $(pwd)/../../packages/supply-and-demand-pilet:/source"
docker run ${volMount} ${args} --rm --name snd-table-checkmarx-runner checkmarx /run.sh

echo "finished"