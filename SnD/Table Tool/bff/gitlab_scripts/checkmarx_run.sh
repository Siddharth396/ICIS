# exit on any error
set -e

rm ./build_tools/checkmarx/source -rf

mkdir ./build_tools/checkmarx/source
cp ./src/main/Authoring/ ./build_tools/checkmarx/source -r
cp ./src/main/Subscriber/ ./build_tools/checkmarx/source -r
cp ./src/main/BusinessLayer/ ./build_tools/checkmarx/source -r
cp ./src/main/Infrastructure/ ./build_tools/checkmarx/source -r

cd ./build_tools/checkmarx

echo "*************************************************************"
echo "******* checkmarx check for test bff"
echo "*************************************************************"

docker-compose down

docker-compose build checkmarx

docker-compose run --rm --name starter-checkmarx-runner checkmarx

# clean up
rm ./build_tools/checkmarx/source -rf
