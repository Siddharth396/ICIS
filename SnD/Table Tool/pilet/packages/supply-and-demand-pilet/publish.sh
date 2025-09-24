#!/bin/bash

# To support publishing into local docker setup via feed service local seeder
if [ -n "$SECRET_FILE_PATH" ]; then
  echo "Loading secret from path $SECRET_FILE_PATH"
  set -o allexport
    source $SECRET_FILE_PATH
  set +o allexport
fi

piletName=supply-and-demand

if [[ $IS_AUTHORING == true ]]
then
    piletName+="-authoring"
fi
echo $piletName

echo "deleting ${piletName}@${SEM_VER} if it already exists"
curl --verbose -i -k -X DELETE $FEED_SERVICE_URL/$piletName/$SEM_VER -H "Authorization: basic $FEED_SERVICE_API_KEY"


echo "publishing to $FEED_SERVICE_URL"
curl --verbose -i -k -F "file=@pilet-package.tgz" $FEED_SERVICE_URL -H "Authorization: basic $FEED_SERVICE_API_KEY"
