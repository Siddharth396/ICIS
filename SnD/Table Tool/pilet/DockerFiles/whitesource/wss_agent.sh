#!/bin/bash 

# This is a copy of 
# https://github.com/whitesource/unified-agent-distribution/raw/master/standAlone/wss_agent.sh
# just with the proxy passed into cUrl

curl -LJO https://github.com/whitesource/unified-agent-distribution/releases/latest/download/wss-unified-agent.jar

curl -LJO https://github.com/whitesource/unified-agent-distribution/raw/master/standAlone/wss-unified-agent.config

java -jar wss-unified-agent.jar "$@"