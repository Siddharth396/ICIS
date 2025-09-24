#!/bin/bash
set -e

# Branch that is being deployed
BRANCH_NAME=$1
PRIVATE_TOKEN=$2
#project-ID on gitlab
PROJECT_ID=$3

# Tag for helm chart
SEM_VERSION=$4


IMAGE_BASEREPO="supdem"
HELM_DOCKER_IMAGE="artifacts.cha.rbxd.ds/${IMAGE_BASEREPO}/snd-charts"
HELM_DOCKER_TAG=${SEM_VERSION}

POLLER_DOCKER_IMAGE=$"${IMAGE_BASEREPO}/snd-eventprocessor-poller"
POLLER_DOCKER_TAG=$5

PROCESSOR_DOCKER_IMAGE=$"${IMAGE_BASEREPO}/processor"
PROCESSOR_DOCKER_TAG=$6
echo "Creating branch on gitlab....."

curl --request POST --header "PRIVATE-TOKEN: ${PRIVATE_TOKEN}" "https://gitlab.cha.rbxd.ds/api/v4/projects/${PROJECT_ID}/repository/branches?branch=${BRANCH_NAME}&ref=main"



echo -e "\nCreating Commit......"


PAYLOAD_CREATE="
{
  \"branch\": \"${BRANCH_NAME}\",
  \"commit_message\": \"${SEM_VERSION}\",
  \"actions\": [
    {
      \"action\": \"update\",
      \"file_path\": \"build.env\",
      \"content\": \"SEM_VERSION=${SEM_VERSION}\r\nIMAGE_BASEREPO=${IMAGE_BASEREPO}\r\nHELM_DOCKER_IMAGE=${HELM_DOCKER_IMAGE}\r\nHELM_DOCKER_TAG=${HELM_DOCKER_TAG}\r\nBRANCH=${BRANCH_NAME}\r\nPOLLER_DOCKER_IMAGE=${POLLER_DOCKER_IMAGE}\r\nPOLLER_DOCKER_TAG=${POLLER_DOCKER_TAG}\r\nPROCESSOR_DOCKER_IMAGE=${PROCESSOR_DOCKER_IMAGE}\r\nPROCESSOR_DOCKER_TAG=${PROCESSOR_DOCKER_TAG}\"
    }
  ]
}
"

curl --request POST --header "PRIVATE-TOKEN: ${PRIVATE_TOKEN}" --header "Content-Type: application/json" \
    --data "$PAYLOAD_CREATE" "https://gitlab.cha.rbxd.ds/api/v4/projects/${PROJECT_ID}/repository/commits" || true
