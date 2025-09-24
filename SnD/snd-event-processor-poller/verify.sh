#!/bin/bash

set -e



# Poller Image tage and helm image should be the same
helmDockerTag=$1
latestPollerTag=$1
# Since Processor is going to be replaced in the nearest feature and no new updates are planned, docker image tag would remain constant
latestProcessorTag="0.1.0-beta.1.258-322"

function LogDeploymentInfo {


    echo "##teamcity[setParameter name='helmTag' value='$3']"
    echo "##teamcity[setParameter name='pollerTag' value='$1']"
    echo "##teamcity[setParameter name='processorTag' value='$2']"

    echo "Latest Poller Image Tag: $1 is set for deployment"
    echo "Latest Processor Image Tag: $2 is set for deployment"
    echo "Latest Chart Image Tag: $3 is set for deployment"

}


  LogDeploymentInfo "$latestPollerTag" "$latestProcessorTag" "$helmDockerTag"
