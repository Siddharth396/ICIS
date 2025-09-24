#!/bin/bash
set -e
echo "$jsonValue"
echo "-------------------------------------------------------------------------------"
echo "Deploying $SEM_VERSION to $ENVIRONMENT"
echo "-------------------------------------------------------------------------------"
WORKFLOW_IDENTIFIER=$(openssl rand -base64 12)
WORKFLOW_IDENTIFIER="price-entry-pilet-${WORKFLOW_IDENTIFIER}"
echo "Workflow Identifier = $WORKFLOW_IDENTIFIER"

echo "Triggering deployment pipeline at $PROJECT_DEPLOYERPATH_URL with the following parameters:"

if [ -z "${IMAGE_REPO}" ]
then
  IMAGE_REPO="artifacts.cha.rbxd.ds/sbmfe/sbmfe-price-entry-pilet${PILET_TYPE}"
fi

if [ -z "${VERIFY_DEPLOY}" ]
then
  VERIFY_DEPLOY="true"
fi

if [ -z "${IMAGE_TAG}"  ]; then
  IMAGE_TAG="${SEM_VERSION}"
fi

repoName="${DEPLOYMENT_REPO}"
echo "Repo Name = $repoName"

CI_PATH="LexisNexis-RBA/dsg-icis-capability-price-entry-pilet"

DATE_UTC=$(date -u +"%Y-%m-%dT%H:%M:%S")
echo "Current UTC Date and Time: $DATE_UTC"
# Trigger the common deployment pipeline
gh workflow run deploy.yml \
    --repo https://github.com/${repoName} \
    --ref main -f ASSETID=$ASSETID \
    -f ENVIRONMENT=$ENVIRONMENT \
    -f IMAGE_TAG=$IMAGE_TAG \
    -f IMAGE_REPO=$IMAGE_REPO \
    -f ENVIRONMENT=$ENVIRONMENT \
    -f FEED_SERVICE_URL=$FEED_SERVICE_URL \
    -f CI_PATH=$CI_PATH -f WORKFLOW_IDENTIFIER=$WORKFLOW_IDENTIFIER

# Get all runs for the workflow
# Loop through each workflow run and fetch workflow id for each workflow run 

workflow_id_status=-1
workflow_id_loop_count=0
while [ $workflow_id_status -eq -1 ]
do  
    WORKFLOW_RUNS=$(gh api repos/${repoName}/actions/runs?created=\>${DATE_UTC})
    WORKFLOW_RUN_ID=$(echo $WORKFLOW_RUNS | jq '.workflow_runs[] | select(.name=="'$WORKFLOW_IDENTIFIER'").id')
    if [ -n "$WORKFLOW_RUN_ID" ]; then
        workflow_id_status=0
        echo "Workflow Run ID = $WORKFLOW_RUN_ID"
        break
    fi
    workflow_id_loop_count=$((workflow_id_loop_count+1))
    if [ $workflow_id_loop_count -eq 100 ]; then
        echo "Fetching the workflow id timed out....."
        workflow_id_status=1
        break
    fi
    echo "waiting for 5 seconds....."
    sleep 5
done

exit_code=-1

VARSUCCESS="success"
VARFAIL="failure"
loop_count=0
mkdir logs

echo "-------------------------------------------------------------------------------"
echo "if you are not patient to wait for the logs at the end, here is a link"
echo "https://github.com/${repoName}/actions/runs/${WORKFLOW_RUN_ID}"
echo "-------------------------------------------------------------------------------"

while [ $exit_code -eq -1  ]
do
    WORKFLOW_RUN=$(gh api repos/${repoName}/actions/runs/$WORKFLOW_RUN_ID)
    
    STATUS=$(echo $WORKFLOW_RUN | jq -r '.conclusion')
    echo "Status = $STATUS"

    if [ "$STATUS" = "$VARSUCCESS" ]; then
        echo "deployment succeeded"
        exit_code=0
        break
    fi

    if [ "$STATUS" = "$VARFAIL" ]; then
        echo "deployment failed"
        exit_code=1
        break
    fi
    loop_count=$((loop_count+1))
    if [ $loop_count -eq 100 ]; then
        echo "deployment timed out"
        exit_code=1
        break
    fi
    echo "waiting for 5 seconds"
    sleep 5
done

{
    gh api \
    -H "Accept: application/vnd.github+json" \
    -H "X-GitHub-Api-Version: 2022-11-28" \
    repos/${repoName}/actions/runs/$WORKFLOW_RUN_ID/logs > logs/logs.zip
    echo "parsing logs"

    unzip -qq logs/logs.zip -d logs/
    last_line=$(tail -n 100 logs/0_Deployment.txt)
    echo "------------------------------------------------------"
    echo "Last 100 lines of 0_Deployment.txt: $last_line"
    echo "------------------------------------------------------"
    # clean up the logs after wards
    rm -rf logs/*
} || {
    echo "An error occurred while fetching or processing the logs., manual read the logs"
}
exit $exit_code
