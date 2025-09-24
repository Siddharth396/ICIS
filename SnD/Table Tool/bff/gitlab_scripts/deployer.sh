IMAGE_REPO="snd/sndctl/${IMAGE_TYPE}-bff"
IMAGE_TAG="${SEM_VERSION}"

#This ACCESS_TOKEN is used for pulling the flux-pipeline status and logs of each job in that pipeline
#It's different from CI_JOB_TOKEN which is actually used to trigger the flux-pipeline
#To be decided if this needs to be a secret or not.
ACCESS_TOKEN=$GITLAB_READONLY_ACCESS_TOKEN

echo "Triggering deployment pipeline at $PROJECT_DEPLOYERPATH_FLUX_URL with the following parameters:"
echo "IMAGE_TAG: $IMAGE_TAG"
echo "IMAGE_REPO: $IMAGE_REPO"
echo "ASSETID: $ASSETID"
echo "ENVIRONMENT: $ENVIRONMENT"
echo "INPUT_USERID: $GITLAB_USER_ID"
echo "INPUT_USERNAME: $GITLAB_USER_NAME"

pipeline_result=$( curl -X -ik --insecure --request POST \
      --form token=$CI_JOB_TOKEN \
      --form ref=master \
      --form "variables[IMAGE_TAG]=$IMAGE_TAG" \
      --form "variables[IMAGE_REPO]=$IMAGE_REPO" \
      --form "variables[ASSETID]=$ASSETID" \
      --form "variables[TAG_TYPE]=$ENVIRONMENT" \
      --form "variables[CLUSTER_VERSION]=$CLUSTER_VERSION" \
      --form "variables[INPUT_USERID]=$GITLAB_USER_ID" \
      --form "variables[INPUT_USERNAME]=$GITLAB_USER_NAME" \
      "${PROJECT_DEPLOYERPATH_FLUX_URL}/trigger/pipeline" )

echo "result: $pipeline_result"

pipeline_id=$(echo $pipeline_result | jq '.id')

if [ $pipeline_id == "" ]
then 
    echo "Failed to trigger deployment pipeline. Exiting."
    exit 1
fi

declare condition=0
declare pipeline_status="failed"
echo "##############Gitlab Pipeline Status ###################"
while [ $condition == 0 ]
do
    pipeline_status=$(curl --insecure --header "PRIVATE-TOKEN: $ACCESS_TOKEN" "${PROJECT_DEPLOYERPATH_FLUX_URL}/pipelines/$pipeline_id" | jq -rc '.status')

    if [ $pipeline_status == "success" ]
    then 
        condition=1
        echo "###############Deployment completed successfully.#####################"
        printf "\n\n\n"
    elif [ $pipeline_status == "failed" ]
    then 
        echo "###############Deployment has failed.###############################"
        condition=1
        printf "\n\n\n"
    else
        echo "Current status: $pipeline_status"
        echo "sleeping for 15 seconds to wait for deployment to be finished."
        sleep 15s
    fi
    printf "\n"
done

curl --insecure --header "PRIVATE-TOKEN: $ACCESS_TOKEN" "${PROJECT_DEPLOYERPATH_FLUX_URL}/pipelines/$pipeline_id/jobs" | jq -c 'sort_by(.started_at) | .[] | {"jobId": .id, "jobName": .name, "jobStatus": .status}' > jobs.txt

printf "\n\n\n\n\n"
printf "#####################Fetching Job Details###############"
printf "\n\n\n\n"
while read job
do
    job_id=$(echo $job | jq '.jobId')
    job_name=$(echo $job | jq '.jobName')
    job_status=$(echo $job | jq '.jobStatus')
    echo "###################Job Details: $job_id##################"
    echo "Job Name: $job_name"
    echo "Job Status: $job_status"
    echo "Job Logs: "
    curl --insecure --header "PRIVATE-TOKEN: $ACCESS_TOKEN" "${PROJECT_DEPLOYERPATH_FLUX_URL}/jobs/$job_id/trace"
    printf "\n\n"
done < jobs.txt

if [ $pipeline_status == "failed" ]
then 
exit 1
fi
