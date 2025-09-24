#!/usr/bin/env bash
echo "WORKING DIRECTORY: ${PWD}"

# get the dependencies up and running
docker-compose -f ./dependencies/docker-compose.yml pull
docker-compose -f ./dependencies/docker-compose.yml up -d --build

# wait for dependencies to be up before running the test
. ./scripts/wait-for-status.sh

echo "Waiting for bff"
waitfor https://localhost:4200/v1/version

echo "Waiting for feedservice"
waitfor http://localhost:8090/api/v1/mfe

echo "*** Getting a list of pilets to publish and run E2E tests for ***"

echo "pulling from $SOURCE_FEED"
piletList=$(curl -s --insecure $SOURCE_FEED | jq '.items[] .name')
echo "*** Pilet list is $piletList ***"
tag=latest
e2eTag=$PiletTag
mkdir results

echo "*** IGNORE_PILETS_REGEXP is $IGNORE_PILETS_REGEXP ***"
piletString="supply-and-demand-pilet"
# for piletString in $piletList
# do
#remove quotes
pilet=$(sed -e 's/^"//' -e 's/"$//' <<<"$piletString")

# echo $pilet
# if [[ $pilet =~ $IGNORE_PILETS_REGEXP ]] ; then
#     echo 'Skipping the pilet'
#     continue
# fi

docker stop $pilet-integration-tests || true && docker rm $pilet-integration-tests || true
docker stop $pilet || true && docker rm $pilet || true
docker stop $pilet-mockbff || true && docker rm $pilet-mockbff || true

# we publish the pilet
echo "Publishing $pilet:$e2eTag"
docker pull artifacts.cha.rbxd.ds/$pilet:$e2eTag
docker run --network host -e FEED_SERVICE_URL=http://localhost:8090/api/v1/mfe -v ${PWD}/localhost/local.env:/pilet/local.env -e SECRET_FILE_PATH=/pilet/local.env --name $pilet artifacts.cha.rbxd.ds/$pilet:$e2eTag || exit -1

# we run the mock bff
echo "Publishing $pilet-mockbff:$e2eTag"
docker pull artifacts.cha.rbxd.ds/$pilet-mockbff:$e2eTag || true # optional
docker run -d --name $pilet-mockbff artifacts.cha.rbxd.ds/$pilet-mockbff:$e2eTag || true # optional

# we are checking the running containers
echo "Displaying the running containers"
docker ps

# run the integration tests container
echo "Running the integration tests - $pilet-e2etest:$e2eTag"
docker pull artifacts.cha.rbxd.ds/$pilet-e2etest:$
docker run --network host -e TEST_ENV=local -e TEST_TAGS=headless-chrome-smoke --name $pilet-integration-tests artifacts.cha.rbxd.ds/$pilet-e2etest:$e2eTag

# copy test assets
echo "Copying test results / assets"

mkdir results/$pilet

docker cp $pilet-integration-tests:/e2e-tests/screenshots/ ./results/$pilet/screenshots || true # optional
docker cp $pilet-integration-tests:/e2e-tests/reports/ ./results/$pilet/allure || true # optional
docker cp $pilet-integration-tests:/e2e-tests/videos/ ./results/$pilet/videos || true # optional

docker stop $pilet-integration-tests || true && docker rm $pilet-integration-tests || true
docker stop $pilet || true && docker rm $pilet || true
docker stop $pilet-mockbff || true && docker rm $pilet-mockbff || true

# done

# stop and remove the container

echo "Tidying up"
docker-compose -f ./docker-compose.test.yml down

echo "Complete"
