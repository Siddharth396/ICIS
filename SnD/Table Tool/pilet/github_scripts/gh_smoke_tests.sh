#!/bin/bash
set -e

DATE_UTC=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
echo "Current UTC Date and Time: $DATE_UTC"

gh workflow run test.yaml \
 --repo LexisNexis-RBA/dsg-icis-snd-table-tool-Automation-suite \
 --ref main --field name=fe-smoke-test

echo "Sleeping for 15 seconds"
sleep 15

WORKFLOW_RUNS=$(gh api "repos/LexisNexis-RBA/dsg-icis-snd-table-tool-Automation-suite/actions/runs?created=>$DATE_UTC")

WORKFLOW_RUN_ID=$(echo $WORKFLOW_RUNS | jq '.workflow_runs[].id')

echo "Workflow Run ID = $WORKFLOW_RUN_ID"

exit_code=-1

VARSUCCESS="success"
VARFAIL="failure"
loop_count=0
mkdir logs

while [ $exit_code -eq -1  ]
do
    WORKFLOW_RUN=$(gh api repos/LexisNexis-RBA/dsg-icis-snd-table-tool-Automation-suite/actions/runs/$WORKFLOW_RUN_ID)
    
    STATUS=$(echo $WORKFLOW_RUN | jq -r '.conclusion')
    echo "Status = $STATUS"

    if [ "$STATUS" = "$VARSUCCESS" ]; then
        echo "Pilet smoke tests succeeded"
        exit_code=0
        break
    fi

    if [ "$STATUS" = "$VARFAIL" ]; then
        echo "Pilet smoke tests failed"
        exit_code=1
        break
    fi
    loop_count=$((loop_count+1))
    if [ $loop_count -eq 100 ]; then
        echo "Pilet smoke tests timed out"
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
    repos/LexisNexis-RBA/dsg-icis-snd-table-tool-Automation-suite/actions/runs/$WORKFLOW_RUN_ID/logs > logs/logs.zip
    echo "parsing logs"

    unzip -qq logs/logs.zip -d logs/
    
    last_line=$(tail -n 100 logs/0_fe-smoke-test.txt)
    echo "------------------------------------------------------"
    echo "Last 100 lines of 0_fe-smoke-test.txt: $last_line"
    echo "------------------------------------------------------"
    # clean up the logs after wards
    rm -rf logs/*
} || {
    echo "An error occurred while fetching or processing the logs., manual read the logs"
}

WORKFLOW_ARTIFACTS=$(gh api repos/LexisNexis-RBA/dsg-icis-snd-table-tool-Automation-suite/actions/artifacts)
ARCHIVE_URL=$(echo $WORKFLOW_ARTIFACTS | jq -r '.artifacts[0].archive_download_url')

echo "Artifacts URL For Pilet Smoke Tests  = $ARCHIVE_URL"

exit $exit_code